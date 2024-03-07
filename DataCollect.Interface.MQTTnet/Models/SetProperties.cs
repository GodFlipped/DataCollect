namespace DataCollect.Interface.MQTTnet.Models
{
    public class SetProperties
    {
        public string deviceId { get; set; }
        public long timestamp { get; set; }
        public string messageId { get; set; }
        public int version { get; set; }
    }
}
