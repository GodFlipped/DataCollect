using DotNetty.Buffers;
using System;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 起始状态消息
    /// </summary>
    public class InitMessageToPlc : NettyClientMessageBody
    {
        public InitMessageToPlc(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            DateTime = byteBuffer.ReadLong();
        }

        public InitMessageToPlc(ushort msgType, DateTime dateTime) : base(msgType)
        {
            MessageLength = 20;
            DateTime = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
        }



        public long DateTime { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteLong(DateTime);
            return byteBuffer;
        }
    }
}
