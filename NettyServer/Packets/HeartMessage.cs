

using DotNetty.Buffers;
using Kengic.Was.Connector.NettyClient.Packets;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    public  class HeartMessage : SorterMessage
    {
        /// <summary>
        /// 心跳消息
        /// </summary>
        public HeartMessage()
        {
        }
        public static ushort ByteLength => 6;

        public HeartMessage(ushort msgType, ushort beatClycle) : base(msgType)
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

        protected internal override bool Decode(IByteBuffer byteBuffer, ref int remainingLength)
        {
            if (!byteBuffer.IsReadable(remainingLength))
            {
                return false;
            }
            var originMessageLength = ByteLength;
            if (remainingLength > 0)
            {
                MessageLength = byteBuffer.ReadUnsignedShort();
                MessageType = byteBuffer.ReadUnsignedShort();
                BeatClycle = byteBuffer.ReadUnsignedShort();
                remainingLength -= originMessageLength;
            }
            return true;
        }

        public ushort BeatClycle { get; set; }
        public string DataContent { get; set; }
    }
}
