using DotNetty.Buffers;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class ErrorMessage : NettyClientMessageBody
    {
        public ErrorMessage(IByteBuffer byteBuffer, int messageLength) : base(byteBuffer)
        {
            if (byteBuffer!=null)
            {
                byte[] data = new byte[messageLength];
                byteBuffer.GetBytes(0, data);
                DataContext = BitConverter.ToString(data).Replace("-", "");
                //设置读取index至末尾
                byteBuffer.SetReaderIndex(byteBuffer.WriterIndex);
            }
            MessageType = 0xFFFF;
        }
        public ErrorMessage(ushort messageType, string dataContent) : base(messageType)
        {
            MessageType = messageType;
            DataContext = dataContent;
        }

        public override IByteBuffer GetByteBuffer()
        {
            throw new NotImplementedException();
        }
    }
}
