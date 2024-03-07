using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttClient.TcpService
{
    public class TcpConnectEventArgs : EventArgs
    {
        public TcpConnectEventArgs(EndPoint remoteEndPoint, string message)
        {
            RemoteEndPoint = remoteEndPoint;
            Message = message;
        }

        public EndPoint RemoteEndPoint { get; set; }
        public string Message { get; set; }
    }
}
