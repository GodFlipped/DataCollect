using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDRuningStatus : NettyClientMessageBody
    {

        public MZDRuningStatus(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            RuningStatus = byteBuffer.ReadUnsignedShort();
        }

        public MZDRuningStatus(ushort msgType, ushort runingStatus) : base(msgType)
        {
            MessageLength = (ushort)(6);
            RuningStatus = runingStatus;
        }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(RuningStatus);
            return byteBuffer;
        }

        public ushort RuningStatus { get; set; }
    }
}
