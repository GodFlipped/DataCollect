using DataCollect.Interface.KgMqttClient.Dtos;
using Furion.DependencyInjection;
using HslCommunication;
using HslCommunication.MQTT;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace DataCollect.Interface.KgMqttClient
{
   public class KgMqttClient : ISingleton
	{
		public MqttClient mqttClient;
		private readonly Timer _reconnectTimer = new Timer();
		public async Task<bool> MqttClientConnection(MqttClientProfile mqttClientProfile)
        {
			bool ret = false;
			MqttConnectionOptions options = new MqttConnectionOptions()
			{
				IpAddress = mqttClientProfile.Ip,
				Port = int.Parse(mqttClientProfile.port),
				ClientId = "5000",
				KeepAlivePeriod = TimeSpan.FromSeconds(100),
			};
			if (!string.IsNullOrEmpty(mqttClientProfile.UserName) || !string.IsNullOrEmpty(mqttClientProfile.password))
			{
				options.Credentials = new MqttCredential(mqttClientProfile.UserName, mqttClientProfile.password);
			}

			mqttClient?.ConnectClose();
			mqttClient = new MqttClient(options);
			mqttClient.LogNet = new HslCommunication.LogNet.LogNetSingle(string.Empty);
			mqttClient.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
			mqttClient.OnMqttMessageReceived += MqttClient_OnMqttMessageReceived;
			mqttClient.OnNetworkError += MqttClient_OnNetworkError;

			OperateResult connect = await mqttClient.ConnectServerAsync();

			if (connect.IsSuccess)
			{

				ret = true;
			}
			else
			{
				mqttClient = null;
				ret = false;
			}
			//添加了定时发送
			//InitializeReconnectTimer();
			return ret;
		}
		private void LogNet_BeforeSaveToFile(object sender, HslCommunication.LogNet.HslEventArgs e)
		{
		
		}
		void InitializeReconnectTimer()
		{

			_reconnectTimer.Elapsed += ReconnectTimer_Elapsed;
			_reconnectTimer.Interval = 500;
			_reconnectTimer.Enabled = true;
		}


		private void ReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			int a = 1;
		}

		private void MqttClient_OnMqttMessageReceived(string topic, byte[] payload)
		{
			try
			{
				
				string msg = Encoding.UTF8.GetString(payload);
				ReturnData returnData = new ReturnData();
				returnData.Subscribe = topic;
				returnData.Data = msg;
				ClientReturnDataEvevt.CreateInstance().Data= returnData;
			}
			catch
			{

			}
		}

		private void MqttClient_OnNetworkError(object sender, EventArgs e)
		{
			// 当网络异常的时候触发，可以在此处重连服务器
			if (sender is MqttClient client)
			{
				// 开始重连服务器，直到连接成功为止
				client.LogNet?.WriteInfo("网络异常，准备10秒后重新连接。");
				while (true)
				{
					// 每隔10秒重连
					System.Threading.Thread.Sleep(10_000);
					client.LogNet?.WriteInfo("准备重新连接服务器...");
					OperateResult connect = client.ConnectServer();
					if (connect.IsSuccess)
					{
						// 连接成功后，可以在下方break之前进行订阅，或是数据初始化操作
						client.LogNet?.WriteInfo("连接服务器成功！");
						break;
					}
					client.LogNet?.WriteInfo("连接失败，准备10秒后重新连接。");
				}
			}
		}
	}
}
