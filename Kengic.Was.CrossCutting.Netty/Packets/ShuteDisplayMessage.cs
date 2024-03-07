using DotNetty.Buffers;
using System.Text;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 起始状态消息
    /// </summary>
    public class ShuteDisplayMessage : NettyClientMessageBody
    {
        public ShuteDisplayMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ShuteId = byteBuffer.ReadUnsignedShort();
            DataLength = byteBuffer.ReadUnsignedShort();
            Content = byteBuffer.ReadString(byteBuffer.ReadableBytes, Encoding.GetEncoding("GB2312"));
        }

        public ShuteDisplayMessage(ushort msgType, ushort shuteId, ushort dataLength,string content) : base(msgType)
        {
            MessageLength = (ushort) (8+ Content.Length);
            ShuteId = shuteId;
            DataLength = dataLength;
            Content = content;
        }

        public ushort ShuteId { get; set; }

        public ushort DataLength { get; set; }

        public string Content { get; set; }
        public ushort Sequence { get; set; }
        public string Cycles { get; set; }
        public bool IsSendForAuto { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(ShuteId);
            byteBuffer.WriteUnsignedShort(DataLength);
            byteBuffer.WriteString(Content, Encoding.GetEncoding("GB2312"));
            return byteBuffer;
        }
    }
}
