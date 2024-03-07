using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 确认消息
    /// </summary>
    public class ConfirmMessage : NettyClientMessageBody
    {
        public ConfirmMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ConfirmSequence = byteBuffer.ReadUnsignedShort();
        }

        public ConfirmMessage(ushort msgType, ushort confirmSequence) : base(msgType)
        {
            MessageLength = 6;
            ConfirmSequence = confirmSequence;
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(ConfirmSequence);
            return byteBuffer;
        }

        public ushort ConfirmSequence { get; set; }
    }
}
