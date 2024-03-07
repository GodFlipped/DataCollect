using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public abstract class NettyClientMessageBody
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public NettyClientMessageBody(IByteBuffer byteBuffer)
        {
            if (byteBuffer!=null)
            {
                MessageLength = byteBuffer.ReadUnsignedShort();
                MessageType = byteBuffer.ReadUnsignedShort();
            }

        }

        public NettyClientMessageBody(ushort messageType)
        {
            MessageType = messageType;
        }


        /// <summary>
        /// 单个body长度
        /// </summary>
        public ushort MessageLength { get; set; }

        public ushort MessageType { get; set; }

        public string DataContext { get; set; }

        public byte[] sendBytes { get; set; }

        public abstract IByteBuffer GetByteBuffer();
    }
}