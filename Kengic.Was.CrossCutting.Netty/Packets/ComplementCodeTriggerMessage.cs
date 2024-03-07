using DotNetty.Buffers;
using System;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 补码触发消息
    /// </summary>
    public class ComplementCodeTriggerMessage : NettyClientMessageBody
    {
        public ComplementCodeTriggerMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            DateTime = byteBuffer.ReadLong();
        }

        public ComplementCodeTriggerMessage(ushort msgType, DateTime dateTime) : base(msgType)
        {
            MessageLength = 12;
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
