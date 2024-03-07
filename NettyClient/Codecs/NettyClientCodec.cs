using DotNetty.Transport.Channels;

namespace Kengic.Was.Connector.NettyClient.Codecs
{
    public class NettyClientCodec : CombinedChannelDuplexHandler<NettyClientDecoder, NettyClientEncoder>
    {
        public NettyClientCodec()
        {
            Init(new NettyClientDecoder(), new NettyClientEncoder());
        }
    }
}
