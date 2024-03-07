using DataCollect.Core.IHttpRequest;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Furion.Localization;
using DataCollect.Core.Entities.Mqtt;
using Furion.DatabaseAccessor;
using System.Linq;
using Furion.FriendlyException;
using Mapster;
using System.Collections.Generic;
using System.Text;
using DataCollect.Application;
using DataCollect.Interface.KgMqttServer.Mqtt;
using DataCollect.Interface.KgMqttServer.Mqtt.Enums;
using DataCollect.Interface.KgMqttServer.Mqtt.Dtos;
using DataCollect.Interface.KgMqttClient;
using DataCollect.Interface.KgMqttClient.Dtos;
using System.Threading.Tasks;
using DataCollect.Application.Service;
using DataCollect.Interface.MQTTnet;
using DataCollect.Interface.TCPServer;
using System;
using DataCollect.Interface.TCPServer.Models;

namespace DataCollect.Api.Controllers
{
    /// <summary>
    /// MQTT标准协议
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ApiDescriptionSettings(ApiGroupConsts.CLIENT_CENTER)]
    public class MqttController : IDynamicApiController
    {
        private readonly IHttp _http;
        private readonly ILogger<MqttController> _log;
        private readonly MqttServerConnect _mqttServerConnect;
        private readonly IRepository<MqttConnect> _mqttConnectRepository;
        private readonly IRepository<CollectPlcData> _collectPlcData;
        private KgMqttClient _kgMqttClient;
        private MQTTClientDome _mQTTClientDome;
        private MQTTServerDome _mQTTServerDome;
        private HandlePlcTaskDome _handlePlcTaskDome;
        private MQTTnetClient _MQTTnetClient;
        private MQTTnetDome _mQTTnetDome;
        private TcpServer _tcpServer;
        private TcpServerDome _tcpServerDome;
        public MqttController(IHttp http, MqttServerConnect mqttServerConnect,
            ILogger<MqttController> log,
            IRepository<MqttConnect> mqttConnectRepository,
            KgMqttClient kgMqttClient,
            MQTTClientDome mQTTClientDome,
            MQTTServerDome mQTTServerDome,
            HandlePlcTaskDome handlePlcTaskDome,
            MQTTnetClient MQTTnetClient,
            MQTTnetDome mQTTnetDome,
            TcpServer tcpServer,
            TcpServerDome tcpServerDome,
            IRepository<CollectPlcData> collectPlcData
            )
        {
            _http = http;
            _mqttServerConnect = mqttServerConnect;
            _log = log;
            _mqttConnectRepository = mqttConnectRepository;
            _kgMqttClient = kgMqttClient;
            _mQTTClientDome = mQTTClientDome;
            _mQTTServerDome = mQTTServerDome;
            _handlePlcTaskDome = handlePlcTaskDome;
            _MQTTnetClient = MQTTnetClient;
            _mQTTnetDome = mQTTnetDome;
            _tcpServer = tcpServer;
            _tcpServerDome = tcpServerDome;
            _collectPlcData = collectPlcData;
        }
        /// <summary>
        /// 启动Mqtt服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("MqttConnectServer")]
        public string MqttConnectServer()
        {
            try
            {
                var mqttConnect = _mqttConnectRepository.DetachedEntities.ToList();
                if (mqttConnect.Count() != 1)
                {
                    throw Oops.Oh(MqttErrorCodes.m1000);
                }
                var mqttData = mqttConnect.Adapt<List<MqttConnectProfile>>().FirstOrDefault();
                if (_mqttServerConnect.MqttConnectServer(mqttData))
                {
                    _mQTTServerDome.ServerDome();
                    _handlePlcTaskDome.HandTaskDome();
                    return L.Text["Mqtt服务创建成功"];
                }
                else
                {
                    return L.Text["Mqtt服务创建失败"];
                }
            }
            catch (System.Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }

        /// <summary>
        /// 关闭Mqtt服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("ServerClose")]
        public string ServerClose()
        {
            try
            {
                _mqttServerConnect.mqttServer.ServerClose();
                _log.LogInformation(L.Text["Mqtt服务端关闭成功"]);
                return L.Text["Mqtt服务端关闭成功"];
            }
            catch (System.Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }

        /// <summary>
        /// 广播所有
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns></returns>
        [HttpGet("PublishAll")]
        public string PublishAll(string data)
        {
            try
            {
                var mqttConnect = _mqttConnectRepository.DetachedEntities.ToList();
                if (mqttConnect.Count() != 1)
                {
                    throw Oops.Oh(MqttErrorCodes.m1000);
                }
                var mqttData = mqttConnect.Adapt<List<MqttConnectProfile>>().FirstOrDefault();
                _mqttServerConnect.mqttServer.PublishAllClientTopicPayload(mqttData.SubscribeName, Encoding.UTF8.GetBytes(data));
                _log.LogInformation(L.Text["Mqtt广播所有发送成功"]);
                return L.Text["Mqtt广播所有发送成功"];
            }
            catch (System.Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }

        /// <summary>
        /// 广播订阅
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns></returns>
        [HttpGet("Publish")]
        public string Publish(string data)
        {
            try
            {
                var mqttConnect = _mqttConnectRepository.DetachedEntities.ToList();
                if (mqttConnect.Count() != 1)
                {
                    throw Oops.Oh(MqttErrorCodes.m1000);
                }
                var mqttData = mqttConnect.Adapt<List<MqttConnectProfile>>().FirstOrDefault();
                _mqttServerConnect.mqttServer.PublishTopicPayload(mqttData.SubscribeName, Encoding.UTF8.GetBytes(data));
                _log.LogInformation(L.Text["Mqtt广播订阅发送成功"]);
                return L.Text["Mqtt广播订阅发送成功"];
            }
            catch (System.Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }
        /// <summary>
        /// 广播订阅
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns></returns>
        [HttpGet("MqttClientTest")]
        public async Task<string> MqttClientTest()
        {
            string ret = "失败";
            try
            {

                MqttClientProfile mqttClientProfile = new MqttClientProfile();
                mqttClientProfile.Ip = "127.0.0.1";
                mqttClientProfile.port = "1883 ";
                mqttClientProfile.ClientId = "test";
                if (await _kgMqttClient.MqttClientConnection(mqttClientProfile))
                {
                    _kgMqttClient.mqttClient.SubscribeMessage("A");
                    ret = "成功";
                    _mQTTClientDome.ClientDome();
                }
                _tcpServer.TcpServerConnect("127.0.0.1","2000",DateTime.Now);
                return ret;
            }
            catch (System.Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }

        /// <summary>
        /// 启动TcpServer
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns></returns>
        [HttpGet("TcpServerTest")]
        public string TcpServerTest()
        {
            string ret = "失败";
            try
            {
                //注册事件
                _mQTTnetDome.ClientDome();
                //启动MQtt客户端
                Task.Run(() => _MQTTnetClient.MqttClientConnectionAsync());
                //启动TcpServer
                //if (_tcpServer.TcpServerConnect("10.63.177.246", "2222", DateTime.Now))
                //{
                //    ret = "成功";
                //    _tcpServer.TcpServiceOnDataMessage += _mQTTnetDome.TcpServerEvent_EventTcpServerReceive;
                //};
                ret = "成功";
                return ret;
            }
            catch (Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }

        /// <summary>
        /// 接收was分拣程序业务
        /// </summary>
        /// <returns></returns>
        [HttpPost("HttpForWas")]
        public string HttpForWas(TcpServerMessage msg)
        {
            string ret = "失败";
            try
            {
                var status = _MQTTnetClient._connectStatus;
                if (status)
                {
                    _mQTTnetDome.TcpServerEvent_EventTcpServerReceive(msg);
                    ret = "成功";
                }
            }
            catch (Exception ex)
            {

                return L.Text["错误"] + ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 京东链接测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("JDMqttConnectServer")]
        public async Task<string> JDMqttConnectServer()
        {
            string ret = "失败";
            try
            {
                ////  //注册事件
                ////_mQTTnetDome.ClientDome();
                ////链接服务器
                //await _MQTTnetClient.MqttClientConnectionAsync();

                ret = "成功";
                return ret;

            }
            catch (System.Exception ex)
            {
                return L.Text["错误"] + ex.Message;
            }
        }

        public bool InsertCollectData(CollectPlcData ScadaData)
        {
            try
            {
                CollectPlcData collectData = new CollectPlcData()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    DeviceCode = ScadaData.DeviceCode,
                    OpcValue = ScadaData.OpcValue,
                    IpAddress = ScadaData.IpAddress,
                    Value = ScadaData.Value,
                    OldValue = ScadaData.OldValue,
                    DeviceNumber = ScadaData.DeviceNumber,
                    DeviceType = ScadaData.DeviceType,
                    ComponentProperty = ScadaData.ComponentProperty,
                    ComponentPropertyType = ScadaData.ComponentPropertyType,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IType = ScadaData.IType,
                    Message = ScadaData.Message
                };

                _collectPlcData.Insert(collectData);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError("insertCollectDataError===>"+ex.Message);
                return false;
            }
            
           
        }

    }
}
