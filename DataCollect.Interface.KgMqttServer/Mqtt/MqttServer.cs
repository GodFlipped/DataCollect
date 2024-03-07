using DataCollect.Interface.KgMqttServer.Mqtt.Dtos;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.Localization;
using HslCommunication;
using HslCommunication.MQTT;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttServer.Mqtt
{
    /// <summary>
    /// Mqtt服务端对象
    /// </summary>
    public class MqttServerConnect : ISingleton
	{
		public MqttServer mqttServer;
		private MqttConnectProfile mqttData;
		private readonly ILogger<MqttServerConnect> _log;
		private long receiveCount = 0;
		public MqttServerConnect( ILogger<MqttServerConnect> log)
        {
			
			_log = log;

		}
		public bool MqttConnectServer(MqttConnectProfile mqttConnect)
        {
			try
			{
				
				mqttData = mqttConnect;
				mqttServer = new MqttServer();
				mqttServer.OnClientApplicationMessageReceive += MqttServer_OnClientApplicationMessageReceive;
				mqttServer.OnClientConnected += MqttServer_OnClientConnected;
				if (mqttData.Ispassword)
				{
					mqttServer.ClientVerification += MqttServer_ClientVerification;
				}

				mqttServer.RegisterMqttRpcApi("Account", this);
				mqttServer.RegisterMqttRpcApi("TimeOut", typeof(HslTimeOut));    // 注册的类的静态方法和静态属性
				mqttServer.ServerStart(int.Parse(mqttData.port));
				mqttServer.LogNet = new HslCommunication.LogNet.LogNetSingle("");
				mqttServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
				mqttServer.RegisterMqttRpcApi("Log", mqttServer.LogNet);
				_log.LogInformation(L.Text["启动服务"]);
				return true;
			}
			catch (Exception ex)
			{
				_log.LogError(L.Text["启动失败"] + ex.Message);
				return false;
			}
		}
		private void MqttServer_OnClientConnected(MqttSession session)
		{
			if (mqttData.IsHeartCheck)
			{
				// 当客户端连接上来时，可以立即返回一些数据内容信息
				mqttServer.PublishTopicPayload(session, "HslMqtt", Encoding.UTF8.GetBytes(mqttData.HeartContent));
			}
		}
		private int MqttServer_ClientVerification(MqttSession mqttSession, string clientId, string userName, string passwrod)
		{
			if (userName == mqttData.UserName && passwrod == mqttData.password)
			{
				return 0; // 成功
			}
			else
			{
				return 5; // 账号密码验证失败
			}
		}
		private void LogNet_BeforeSaveToFile(object sender, HslCommunication.LogNet.HslEventArgs e)
		{
			_log.LogInformation(e.HslMessage.ToString() );
		}


		private void MqttServer_OnClientApplicationMessageReceive(MqttSession session, MqttClientApplicationMessage message)
		{
			if (message.Topic == "ndiwh是本地AIHDniwd")   // 用户客户端的压力测试
			{
				mqttServer.PublishTopicPayload(session, message.Topic, message.Payload);
			}

			string msg = Encoding.UTF8.GetString(message.Payload);
			ReturnData returnData = new ReturnData();
			returnData.Subscribe = message.Topic;
			returnData.Data = msg;
			returnData.CliendId = message.ClientId;
			ServerReturnDataEvevt.CreateInstance().Data = returnData;

		

		}
	}
}
