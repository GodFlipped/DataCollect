using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System.Text;

namespace Kengic.Was.Connector.NettyClient.Codecs
{
    public class NettyClientEncoder : MessageToByteEncoder<NettyClientMessage>
    {
        protected override void Encode(IChannelHandlerContext context, NettyClientMessage message, IByteBuffer output)
        {
            if (message.nettyClientMessageBodies[0].MessageType == 0)
            {
                var sendBytes = message.nettyClientMessageBodies[0].sendBytes;
                var byteBuffer = Unpooled.Buffer();
                byteBuffer.WriteBytes(sendBytes);
                output.WriteBytes(byteBuffer);
            }
            else
            {
                output.WriteUnsignedShort(message.Start);
                output.WriteUnsignedShort(message.GetTotalLength());
                output.WriteInt(message.Sequence);
                output.WriteByte(message.XOR);
                output.WriteLong(message.TimeStamp);
                output.WriteString(message.LineNo,Encoding.ASCII);
                foreach (var complementMessageBody in message.nettyClientMessageBodies)
                {
                    output.WriteBytes(complementMessageBody.GetByteBuffer());
                }
            }
        }
    }
}
