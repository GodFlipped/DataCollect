using DataCollect.Interface.KgMqttClient.TcpService;
using Furion.DependencyInjection;

namespace DataCollect.Interface.TCPServer.EventTrigger
{
    public delegate void DelegateTcpServerEventInformation(NetworkDataEventArgs e,object Data);
    public class TcpServertEvent : ISingleton
    {
        public event DelegateTcpServerEventInformation EventTcpServerEventInformation;
        public void OnEventReturnData(NetworkDataEventArgs e, object Data)
        {
            if (EventTcpServerEventInformation != null)
            {
                EventTcpServerEventInformation(e, Data);
            }
        }
    }
}
