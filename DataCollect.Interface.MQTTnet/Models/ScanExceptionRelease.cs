using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet.Models
{
  public  class ScanExceptionRelease
    {
        [JsonProperty("create-time")]
        public string createTime { get; set; }

        [JsonProperty("alarm-no")]
        public string alarmNo { get; set; }

        [JsonProperty("error-code")]
        public string errorCode { get; set; }

        [JsonProperty("component-no")]
        public string componentNo { get; set; }
    }
}
