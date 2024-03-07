using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class ShuteMessage : NettyClientMessageBody
    {
        public ShuteMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            Shute = byteBuffer.ReadUnsignedShort();
        }
        public ShuteMessage(ushort messageType, ushort shute) : base(messageType)
        {
            MessageLength = (ushort)(6);
            Shute = shute;
        }
        
        public ushort Shute { get; set; }
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(Shute);
            return byteBuffer;
        }
    }
}
