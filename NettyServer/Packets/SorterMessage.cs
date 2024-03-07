using DotNetty.Buffers;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    public abstract class SorterMessage
    {
        /// <summary>
        /// 主消息
        /// </summary>
        /// <summary>
        /// 消息体
        /// </summary>
        public SorterMessage()
        {
        }

        public SorterMessage(ushort messageType)
        {
            MessageType = messageType;
        }

        protected internal abstract bool Decode(IByteBuffer byteBuffer, ref int remainingLength);

        /// <summary>
        /// 单个body长度
        /// </summary>
        public ushort MessageLength { get; set; }

        public ushort MessageType { get; set; }

        public abstract IByteBuffer GetByteBuffer();
    }
}
