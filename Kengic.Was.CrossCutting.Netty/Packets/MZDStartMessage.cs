using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDStartMessage : NettyClientMessageBody
    {

        public MZDStartMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            StartOrClose = byteBuffer.ReadUnsignedShort();
        }

        public MZDStartMessage(ushort msgType, ushort startOrClose) : base(msgType)
        {
            MessageLength = (ushort)(6);
            StartOrClose = startOrClose;
        }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(StartOrClose);
            return byteBuffer;
        }

        public ushort StartOrClose { get; set; }
    }
}
