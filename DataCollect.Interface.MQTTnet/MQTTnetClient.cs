using DataCollect.Interface.MQTTnet.EventTrigger;
using DataCollect.Interface.MQTTnet.Models;
using Furion.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet
{
    public class MQTTnetClient : ISingleton
    {
        public IManagedMqttClient managedClient = null;
        public string clientId = "dt1i73gac4400";
        public ILogger _logger;
        private MQTTnetEvent _mQTTnetEvent;
        //MQTT连接状态
        public bool _connectStatus;
        public MQTTnetClient(ILogger<MQTTnetClient> logger, MQTTnetEvent mQTTnetEvent)
        {
            _logger = logger;
            _mQTTnetEvent = mQTTnetEvent;
            _connectStatus = false;
        }

        /// <summary>
        /// MQTT链接服务器
        /// </summary>
        /// <returns></returns>
        public async Task MqttClientConnectionAsync()
        {
            try
            {
                if (managedClient != null)
                {
                    await managedClient.StopAsync();
                }

                string basePath = AppContext.BaseDirectory;
                var factory = new MqttFactory();
                managedClient = factory.CreateManagedMqttClient();
                var caCert = new X509Certificate2(basePath+@"ca/ca.crt");
                var clientCert = new X509Certificate2(basePath+ @"ca/certificate.pfx");
                
                string password = null;

                var managerOptions = new ManagedMqttClientOptionsBuilder()
                   .WithClientOptions(new MqttClientOptionsBuilder()
                       .WithCleanSession(true)
                       .WithClientId(clientId)
                       .WithCredentials(clientId, password)
                       .WithCommunicationTimeout(TimeSpan.FromSeconds(10))
                       .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                       .WithTcpServer("emqx.thingtalk.jdl.com", 2000)
                       .WithProtocolVersion(MqttProtocolVersion.V500)
                       .WithTls(new MqttClientOptionsBuilderTlsParameters()
                       {
                           UseTls = true,
                           SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                           CertificateValidationHandler = (certContext) =>
                           {
                               var chain = new X509Chain();
                               chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                               chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                               chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                               chain.ChainPolicy.VerificationTime = DateTime.Now;
                               chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
                               chain.ChainPolicy.CustomTrustStore.Add(caCert);
                               chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                               var x5092 = new X509Certificate2(certContext.Certificate);
                               return chain.Build(x5092);
                           },
                           Certificates = new List<X509Certificate>()
                           {
                            caCert, clientCert
                           }
                       })
                       .Build())
                   .Build();

                RegisterMqttHandler();

                await managedClient.StartAsync(managerOptions);
            }
            catch (Exception ex)
            {
                _connectStatus = false;
                _logger.LogInformation("未知异常" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public int DateTimeToInt()
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0, 0)); // 当地时区
            var timeStamp = (int)(DateTime.Now.ToUniversalTime() - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        public void RegisterMqttHandler()
        {
            managedClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(
             (e) =>
             {
                 _logger.LogInformation(e + "连接成功");
                 //订阅消息
                 Task.Run(async () =>
                 {
                     await managedClient.SubscribeAsync(new MqttTopicFilterBuilder()
                                        .WithTopic("$iot/v1/device/"+ clientId + "/events/online").Build(),
                                        new MqttTopicFilterBuilder()
                                        .WithTopic("$iot/v1/device/" + clientId + "/properties/post").Build(),
                                         new MqttTopicFilterBuilder()
                                        .WithTopic("$iot/v1/device/" + clientId + "/events/post").Build(),
                                         new MqttTopicFilterBuilder()
                                        .WithTopic("$iot/v1/device/" + clientId + "/thing-model/post").Build());
                     //发送设备上线通知
                     var onLineMessage = new ThinkModelVersion
                     {
                         deviceId = clientId,
                         timestamp = DateTimeToInt(),
                         messageId = Guid.NewGuid().ToString("N"),
                         thingModel = new ThingModel
                         {
                             id = "urn:user-spec-v1:thing-model:pb81ca650a021000:3305151c747139804f44d976efa3813f",
                             version = "V5.0.6"
                         }
                     };



                     var onLineJson = JsonConvert.SerializeObject(onLineMessage);
                     var message = new MqttApplicationMessageBuilder()
                         .WithTopic("$iot/v1/device/" + clientId + "/thing-model/post")
                         .WithPayload(onLineJson)
                         .Build();
                     await managedClient.PublishAsync(message, CancellationToken.None);




                     _connectStatus = true;
                     

                 });
             });

            managedClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(
                (e) =>
                {
                    _logger.LogInformation(e + "断开链接");
                    _connectStatus = false;
                    //尝试重连
                    Task.Run(async () => { await MqttClientConnectionAsync(); });
                });


            managedClient.ApplicationMessageProcessedHandler = new ApplicationMessageProcessedHandlerDelegate(
                (e) =>
                {
                   // _logger.LogInformation("下发的消息" + Encoding.UTF8.GetString(e.ApplicationMessage.ApplicationMessage.Payload));
                });

            managedClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(
                (e) =>
                {
                    string Data = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    //接收到的消息
                    //_logger.LogInformation("收到的消息" + Data);
                    //将消息以事件的形式发布出去
                    _mQTTnetEvent.OnEventReturnData(e, Data);
                });
        }
    }
}
