using DotNetty.Buffers;
using System.Text;
using Kengic.Was.Connector.NettyClient.Packets;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    public class StartResponseMessage : NettyClientMessageBody
    {
        /// <summary>
        /// 起始响应消息
        /// </summary>
        //接收客户端消息
        public StartResponseMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            SiteNo = byteBuffer.ReadString(10, Encoding.ASCII);
            SupplierNo = byteBuffer.ReadString(4, Encoding.ASCII);
            ProductType = byteBuffer.ReadByte();
        }
        //发送消息给客户端
        public StartResponseMessage(ushort msgType, string siteNo, string supplierNo, byte productType) : base(msgType)
        {
            SiteNo = siteNo;
            MessageLength = 19;
            SupplierNo = supplierNo;
            ProductType = productType;
        }


        public string SiteNo { get; set; }

        public string SupplierNo { get; set; }

        public byte ProductType { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteString(SiteNo, Encoding.ASCII);
            byteBuffer.WriteString(SupplierNo, Encoding.ASCII);
            byteBuffer.WriteByte(ProductType);
            return byteBuffer;
        }

    }
}
