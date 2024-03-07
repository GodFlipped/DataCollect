

using DotNetty.Buffers;
using Kengic.Was.CrossCuttings.Netty.Packets;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public  class HeartBeatMessage : NettyClientMessageBody
    {
        /// <summary>
        /// 心跳消息
        /// </summary>
        public HeartBeatMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            BeatClycle = byteBuffer.ReadUnsignedShort();
        }

        public HeartBeatMessage(ushort msgType, ushort beatClycle) : base(msgType)
        {
            BeatClycle = beatClycle;
            MessageLength = 6;

        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(BeatClycle);
            return byteBuffer;
        }

        public ushort BeatClycle { get; set; }
    }
}
