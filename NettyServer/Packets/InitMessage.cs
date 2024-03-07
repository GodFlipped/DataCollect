using DotNetty.Buffers;
using System;
using System.Text;
using Kengic.Was.Connector.NettyClient.Packets;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    /// <summary>
    /// 起始状态消息
    /// </summary>
    public class InitMessage : NettyClientMessageBody
    {
        public InitMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            DateTime = byteBuffer.ReadLong();
        }

        public InitMessage(ushort msgType, DateTime dateTime) : base(msgType)
        {
            MessageLength = 26;
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
