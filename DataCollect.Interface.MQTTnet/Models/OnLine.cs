using Newtonsoft.Json;

namespace DataCollect.Interface.MQTTnet.Models
{
    public class OnLine
    {
        public string deviceId { get; set; }
        public int timestamp { get; set; }
        public string messageId { get; set; }
        [JsonProperty("event")]
        public string events { get; set; }
        public ThingModel thingModel { get; set; }
    }
}
