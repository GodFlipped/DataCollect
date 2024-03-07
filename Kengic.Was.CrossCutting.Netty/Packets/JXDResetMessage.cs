using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class JXDResetMessage : NettyClientMessageBody
    {
        public JXDResetMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ResetType = byteBuffer.ReadUnsignedShort();
        }
        public JXDResetMessage(ushort messageType, ushort resetType) : base(messageType)
        {
            MessageLength = (ushort)(6);
            ResetType = resetType;
        }
        
        public ushort ResetType { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(ResetType);
            return byteBuffer;
        }
    }
}
