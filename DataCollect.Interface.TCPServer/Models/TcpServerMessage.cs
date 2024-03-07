using System;

namespace DataCollect.Interface.TCPServer.Models
{
    public class TcpServerMessage
    {
        public string Id { get; set; }

        public string ObjectToHandle { get; set; }

        public MessageType MessageType { get; set; }

        public string TackingId { get; set; }

        public string Results { get; set; }
        public string ScannerNo { get; set; }
        public string InductNo { get; set; }
        public string RequestChuteNo { get; set; }

        public string ChuteNo { get; set; }

        public string SupplyType { get; set; }

        public int CycleTime { get; set; }

        public string CarrierNo { get; set; }

        public float Weight { get; set; }

        public int Length { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime InductTime { get; set; }

        public DateTime ScannerTime { get; set; }
    }

    public enum MessageType
    {
        Default = 0,
        Heartbeat = 201,
        ScanDataPush = 301,
        ResultDataPush = 302,
    }
}
