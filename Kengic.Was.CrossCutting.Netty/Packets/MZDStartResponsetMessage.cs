using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDStartResponsetMessage : NettyClientMessageBody
    {
        //解码需要 这里传输的是二进制字节 服务端接收
        public MZDStartResponsetMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            StartResult = byteBuffer.ReadUnsignedShort();
        }

        //编码需要 这里把需要发送消息转换成二进制 服务端发送
        public MZDStartResponsetMessage(ushort msgType, ushort startResult) : base(msgType)
        {
            MessageLength = (ushort)(6);
            StartResult = startResult;
        }

        public ushort StartResult { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(StartResult);
            return byteBuffer;
        }

    }
}

