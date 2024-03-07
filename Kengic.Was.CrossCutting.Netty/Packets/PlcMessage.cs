using DotNetty.Buffers;
using System;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class PlcMessage : NettyClientMessageBody
    {
        public PlcMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            MessageType = 0x0000;
        }
        public PlcMessage(ushort messageType, string dataContent) : base(messageType)
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
