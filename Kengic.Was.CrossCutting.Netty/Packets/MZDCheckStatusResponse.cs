using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDCheckStatusResponse : NettyClientMessageBody
    {

        public MZDCheckStatusResponse(IByteBuffer byteBuffer) : base(byteBuffer)
        {
        }

        public MZDCheckStatusResponse(ushort msgType, ushort runingStatus) : base(msgType)
        {
            MessageLength = (ushort)(6);
            MessageType = msgType;
            RuningStatus = runingStatus;
        }
        
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(RuningStatus);
            return byteBuffer;
        }
        public ushort RuningStatus { get; set; }

    }
}
