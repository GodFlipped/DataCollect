using Newtonsoft.Json;
using System;

namespace DataCollect.Interface.MQTTnet.Models
{
    public class ScanInfoModel
    {
        [JsonProperty("package-no")]
        public string packageNo { get; set; }

        [JsonProperty("scan-time")]
        public string scanTime { get; set; }

        

        [JsonProperty("component-no")]
        public string inductNo { get; set; }

        [JsonProperty("supply-type")]
        public string supplyType { get; set; }

        [JsonProperty("scanner-type")]
        public string scannerType { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
    }
}
