using DataCollect.Interface.KgMqttClient.TcpService;
using DataCollect.Interface.TCPServer;
using DataCollect.Interface.TCPServer.EventTrigger;
using Furion.DependencyInjection;

namespace DataCollect.Application.Service
{
    public class TcpServerDome : ITransient
    {
        private TcpServertEvent _tcpServertEvent;
        private TcpServer _tcpServer;
        public TcpServerDome(TcpServertEvent tcpServertEvent, TcpServer tcpServer)
        {
            _tcpServertEvent = tcpServertEvent;
            _tcpServer = tcpServer;
        }
        public void ClientDome()
        {
            _tcpServertEvent.EventTcpServerEventInformation += _tcpServertEvent_EventTcpServerEventInformation;
        }

        /// <summary>
        /// 业务写在此方法
        /// </summary>
        /// <param name="e"></param>
        /// <param name="Data"></param>
        private void _tcpServertEvent_EventTcpServerEventInformation(NetworkDataEventArgs e, object Data)
        {
            //收到的消息
            var aa = Data;

        }
    }
}
