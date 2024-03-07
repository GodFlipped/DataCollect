using DotNetty.Buffers;

namespace Kengic.Was.Connector.NettyClient.Codecs
{
    public interface ICodec
    {
        void Encode(IByteBuffer byteBuffer);
        bool Decode(IByteBuffer byteBuffer, ref int remainingLength);
    }
}
