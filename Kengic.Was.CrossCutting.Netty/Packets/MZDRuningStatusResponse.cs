using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDRuningStatusResponse : NettyClientMessageBody
    {

        public MZDRuningStatusResponse(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            RuningStatus = byteBuffer.ReadUnsignedShort();
            Result = byteBuffer.ReadUnsignedShort();
        }

        public MZDRuningStatusResponse(ushort msgType, ushort runingStatus, ushort result) : base(msgType)
        {
            MessageLength = (ushort)(8);
            RuningStatus = runingStatus;
            Result = result;
        }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(RuningStatus);
            byteBuffer.WriteUnsignedShort(Result);
            return byteBuffer;
        }

        public ushort RuningStatus { get; set; }
        public ushort Result { get; set; }
    }
}
