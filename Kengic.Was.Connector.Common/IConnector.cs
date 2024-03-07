using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.Connector.Common
{
    public interface IConnector
    {
        string Id { get; set; }
        ConcurrentDictionary<string, object> ReceiveDictionary { get; set; }
        ConcurrentDictionary<string, object> ReceiveStatusDictionary { get; set; }
        ConcurrentDictionary<string, object> ReceiveDataDictionary { get; set; }
        bool RecSendMsgStatus { get; set; }
        bool ConnectStatus { get; set; }
        bool AlarmActiveStatus { get; set; }
        bool InitializeStatus { get; set; }

        bool Initialize();
        bool Connect();
        bool DisConnect();
        bool SendMessage(List<string> messageList);
        bool SendMessage(string ipMessage, object message);
        bool SendMessage(object message);
        bool SendByte(ConcurrentQueue<byte[]> sendBuffer);
    }
}
