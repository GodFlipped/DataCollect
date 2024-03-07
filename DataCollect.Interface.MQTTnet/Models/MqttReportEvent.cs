using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet.Models
{
   public class MqttReportEvent : PostEvent
    {
        public List<ThingModelEvent> events { get; set; }
    }
}
