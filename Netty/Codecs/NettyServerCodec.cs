using DotNetty.Transport.Channels;

namespace Kengic.Was.Connector.NettyServer.Codecs
{
    public class NettyServerCodec : CombinedChannelDuplexHandler<NettyServerDecoder, NettyServerEncoder>
    {
        public NettyServerCodec()
        {
            Init(new NettyServerDecoder(), new NettyServerEncoder());
        }
    }
}
