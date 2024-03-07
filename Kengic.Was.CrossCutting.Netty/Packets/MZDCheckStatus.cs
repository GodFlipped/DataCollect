using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDCheckStatus : NettyClientMessageBody
    {

        public MZDCheckStatus(IByteBuffer byteBuffer) : base(byteBuffer)
        {
        }

        public MZDCheckStatus(ushort msgType, ushort runingStatus) : base(msgType)
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
