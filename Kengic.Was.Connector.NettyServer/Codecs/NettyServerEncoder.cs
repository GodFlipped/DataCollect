using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCuttings.Netty.Packets;

namespace Kengic.Was.Connector.NettyCheckeServer.Codecs
{
    public class NettyServerEncoder : MessageToByteEncoder<NettyClientMessage>
    {
        protected override void Encode(IChannelHandlerContext context, NettyClientMessage message, IByteBuffer output)
        {
            if (message.nettyClientMessageBodies[0].MessageType==0)
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
                //output.WriteUnsignedShort(message.Sequence);
                //output.WriteUnsignedShort(message.Version);

                foreach (var complementMessageBody in message.nettyClientMessageBodies)
                {
                    output.WriteBytes(complementMessageBody.GetByteBuffer());
                }
            }
        }
    }
}
