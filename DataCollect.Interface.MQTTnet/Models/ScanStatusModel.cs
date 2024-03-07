using Newtonsoft.Json;

namespace DataCollect.Interface.MQTTnet.Models
{
    public class ScanStatusModel
    {
        public string code { get; set; }
        [JsonProperty("scan-time")]
        public string scanTime { get; set; }
        [JsonProperty("scan-status")]
        public string status { get; set; }
    }
}
