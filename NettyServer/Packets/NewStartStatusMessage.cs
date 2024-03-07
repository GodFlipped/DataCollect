using DotNetty.Buffers;
using Kengic.Was.Connector.NettyClient.Packets;
using System;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    /// <summary>
    /// 新起始状态消息
    /// </summary>
    public class NewStartStatusMessage : NettyClientMessageBody
    {
        public NewStartStatusMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            DateTime = byteBuffer.ReadLong();
        }

        public NewStartStatusMessage(ushort msgType, DateTime dateTime) : base(msgType)
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
