using Furion.DependencyInjection;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet.EventTrigger
{
    public delegate void DelegateMQTTnetEventInformation(MqttApplicationMessageReceivedEventArgs e,string Data);
    public class MQTTnetEvent : ISingleton
    {
        public event DelegateMQTTnetEventInformation EventMQTTnetEventInformation;
        public void OnEventReturnData(MqttApplicationMessageReceivedEventArgs e, string Data)
        {
            if (EventMQTTnetEventInformation != null)
            {
                EventMQTTnetEventInformation(e, Data);
            }

        }
    }
}
