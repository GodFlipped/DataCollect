using Newtonsoft.Json;

namespace DataCollect.Interface.MQTTnet.Models
{
    public class ThinkModelVersion
    {
        public string deviceId { get; set; }
        public int timestamp { get; set; }
        public string messageId { get; set; }
        [JsonProperty("thing-model")]
        public ThingModel thingModel { get; set; }

    }
}
