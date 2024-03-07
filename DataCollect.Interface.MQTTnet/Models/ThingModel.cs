using Newtonsoft.Json;

namespace DataCollect.Interface.MQTTnet.Models
{
    public class ThingModel
    {
        public string id { get; set; }
        [JsonProperty("thing-model-version")]
        public string version { get; set; }
    }
}
