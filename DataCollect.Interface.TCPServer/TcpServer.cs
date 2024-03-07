using DataCollect.Interface.KgMqttClient.TcpService;
using DataCollect.Interface.TCPServer.EventTrigger;
using DataCollect.Interface.TCPServer.Models;
using Furion.DependencyInjection;
using Furion.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace DataCollect.Interface.TCPServer
{
    public class TcpServer : ISingleton
    {
        public delegate void ReceiveMessageEventHandler(object sender, object e);

        private TcpServices _tcpService;
        private TcpServertEvent _tcpServertEvent;
        private ScsHelperForKuaishou _scsHelper = new ScsHelperForKuaishou();
        public event ReceiveMessageEventHandler TcpServiceOnDataMessage;

        public ILogger _logger;
        private byte[] _beforeBytes = { };
        public bool ConnectStatus { get; set; }
        private DateTime _theLastConnectTime;

        public TcpServer(ILogger<TcpServer> logger, TcpServertEvent tcpServertEvent)
        {
            _logger = logger;
            _tcpServertEvent = tcpServertEvent;
        }

        public bool TcpServerConnect(string socketIpAppSetting, string socketPortAppSetting, DateTime theLastConnectTime)
        {
            try
            {
                _theLastConnectTime = theLastConnectTime;
                var socketServerIpAddress = IPAddress.Parse(socketIpAppSetting);
                var socketEndPoint = int.Parse(socketPortAppSetting);
                _tcpService = new TcpServices();
                _tcpService.TcpStart(socketServerIpAddress, socketEndPoint);
                _tcpService.ClientGot += TcpServiceOnClientGot;
                _tcpService.DataGot += TcpServiceOnDataGot;
                _tcpService.ClientMiss += TcpServiceClientMiss;
                _logger.LogInformation(L.Text["启动服务"]);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(L.Text["启动失败"] + ex.Message);
                return false;
            }
        }

        private void TcpServiceClientMiss(object sender, TcpConnectEventArgs networkDataEventArgs)
        {
            ConnectStatus = false;
            _logger.LogInformation(L.Text["TcpServer"] + networkDataEventArgs.RemoteEndPoint.ToString() + "::::" + networkDataEventArgs.Message);
        }

        private void TcpServiceOnDataGot(object sender, NetworkDataEventArgs networkDataEventArgs)
        {
            var infoBytes = networkDataEventArgs.Data;

            byte[] vipMessageBytes = { };
            if (_beforeBytes.Length > 0)
            {
                vipMessageBytes = vipMessageBytes.Concat(_beforeBytes).ToArray();
                vipMessageBytes = vipMessageBytes.Concat(infoBytes).ToArray();
            }
            else
            {
                vipMessageBytes = infoBytes;
            }
            var messageDicts = new ConcurrentDictionary<string, string>();
            _scsHelper.GetReceiveJsonMessageForClient(vipMessageBytes, ref messageDicts, ref _beforeBytes);
            if (messageDicts.Count > 0)
            {
                foreach (var messageDict in messageDicts)
                {
                    var palletMessage = JsonConvert.DeserializeObject<TcpServerMessage>(messageDict.Value);
                    if (palletMessage.MessageType == MessageType.Heartbeat)
                    {
                        continue;
                    }
                    else
                    {
                        TcpServiceOnDataMessage.Invoke(this, palletMessage);
                        //_tcpServertEvent.OnEventReturnData(networkDataEventArgs, palletMessage);
                    }
                }
                _theLastConnectTime = DateTime.Now;
            }
        }

        //客户端连接成功
        private void TcpServiceOnClientGot(object sender, TcpConnectEventArgs tcpConnectEventArgs)
        {
            ConnectStatus = true;
            _logger.LogInformation(L.Text["TcpServer"] + tcpConnectEventArgs.RemoteEndPoint.ToString()+"::::"+tcpConnectEventArgs.Message);
        }

        public bool SendMessage(string scsMessage)
        {
            try
            {
                var Id = Guid.NewGuid().ToString("N") + DateTime.Now.ToString("ssfff");
                if (_scsHelper != null)
                {
                    byte[] scsMessageBytes = _scsHelper.ConvertStringToBytes(scsMessage);

                    var startByte = new byte[] { 2 };
                    var endByte = new byte[] { 3 };

                    var sendMessageBytes =
                        startByte.Concat(scsMessageBytes).Concat(endByte).ToArray();
                    _tcpService.Broadcast(Id, sendMessageBytes);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(L.Text["发送消息异常"] + ex.Message);
                return false;
            }
        }

        //TODO 增减重连机制
        public void RunThread()
        {
            while (true)
            {
                //如果30秒未收到客户端的任何信息则断开重连
                if (DateTime.Now.AddSeconds(-30) >= _theLastConnectTime && ConnectStatus)
                {
                    _tcpService.Stop();
                }
            }
        }
    }
}
