using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttClient.TcpService
{
    public class NetworkDataEventArgs : EventArgs
    {
        public NetworkDataEventArgs(string dataId, EndPoint remoteEndPoint, byte[] data)
        {
            DataId = dataId;
            RemoteEndPoint = remoteEndPoint;
            Data = data;
        }

        public string DataId { get; set; }
        public EndPoint RemoteEndPoint { get; set; }
        public byte[] Data { get; set; }
    }
}
