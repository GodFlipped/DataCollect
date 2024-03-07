using Newtonsoft.Json;
using System;

namespace DataCollect.Interface.MQTTnet.Models
{
    public class TaskStatusModel
    {
        [JsonProperty("package-no")]
        public string packageNo { get; set; }
        [JsonProperty("distinct-supply-no")]
        public string distinctSupplyNo { get; set; }
        [JsonProperty("induction-no")]
        public string inductionNo { get; set; }

        [JsonProperty("car-no")]
        public string carNo { get; set; }

        [JsonProperty("logical-area-no")]
        public string inductGroup { get; set; }

        [JsonProperty("package-length")]
        public int length { get; set; }
        [JsonProperty("package-width")]
        public int width { get; set; }
        [JsonProperty("package-height")]
        public int heigh { get; set; }
        [JsonProperty("package-weight")]
        public float weight { get; set; }
        [JsonProperty("scan-time")]
        public string scanTime { get; set; }

        [JsonProperty("scanner-type")]
        public string scannerType { get; set; }

        [JsonProperty("scanner-no")]
        public string scannerNo { get; set; }

        [JsonProperty("supply-type")]
        public string supplyType { get; set; }
        [JsonProperty("sort-time")]
        public string sortTime { get; set; }
        [JsonProperty("destination-chute-no")]
        public string requestChuteNo { get; set; }
        [JsonProperty("chute-no")]
        public string chuteNo { get; set; }
        [JsonProperty("chute-type")]
        public string chuteType { get; set; }
        [JsonProperty("execute-count")]
        public int executeCount { get; set; }
        [JsonProperty("sort-status-code")]
        public string result { get; set; }
        [JsonProperty("sort-status-desc")]
        public string resultDescription { get; set; }
    }
}
