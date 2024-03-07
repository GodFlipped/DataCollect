namespace DataCollect.Interface.MQTTnet.Models
{
    public class PostEvent
    {
        public string deviceId { get; set; }
        public long timestamp { get; set; }
        public string messageId { get; set; }
    }
}
