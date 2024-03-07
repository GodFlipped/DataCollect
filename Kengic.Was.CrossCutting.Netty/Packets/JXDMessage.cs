using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class JXDMessage : NettyClientMessageBody
    {
        public JXDMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {

        }
        public JXDMessage(ushort messageType) : base(messageType)
        {
            MessageLength = (ushort)(4);
        }
        
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            return byteBuffer;
        }
    }
}
