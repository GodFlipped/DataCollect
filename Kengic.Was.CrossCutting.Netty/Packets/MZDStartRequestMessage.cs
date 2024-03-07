using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDStartRequestMessage : NettyClientMessageBody
    {
        //解码需要 这里传输的是二进制字节 服务端接收
        public MZDStartRequestMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            StartRequest = byteBuffer.ReadUnsignedShort();
        }

        //编码需要 这里把需要发送消息转换成二进制 服务端发送
        public MZDStartRequestMessage(ushort msgType, ushort startRequest) : base(msgType)
        {
            MessageLength = (ushort)(6);
            StartRequest = startRequest;
        }

        public ushort StartRequest { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(StartRequest);
            return byteBuffer;
        }

    }
}

