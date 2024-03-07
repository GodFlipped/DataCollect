using DotNetty.Buffers;
using System;
using System.Text;
using Kengic.Was.Connector.NettyClient.Packets;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    /// <summary>
    /// 起始状态消息
    /// </summary>
    public class StartStatusMessage : NettyClientMessageBody
    {
        public StartStatusMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            SiteNo = byteBuffer.ReadString(10, Encoding.ASCII);
            SupplierNo = byteBuffer.ReadString(4, Encoding.ASCII);
            DateTime = byteBuffer.ReadLong();
        }

        public StartStatusMessage(ushort msgType, string siteNo, string supplierNo, DateTime dateTime) : base(msgType)
        {
            SiteNo = siteNo;
            MessageLength = 26;
            SupplierNo = supplierNo;
            DateTime = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
        }


        public string SiteNo { get; set; }

        public string SupplierNo { get; set; }

        public long DateTime { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteString(SiteNo, Encoding.ASCII);
            byteBuffer.WriteString(SupplierNo, Encoding.ASCII);
            byteBuffer.WriteLong(DateTime);
            return byteBuffer;
        }
    }
}
