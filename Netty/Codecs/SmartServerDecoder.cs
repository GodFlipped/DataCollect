using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Kengic.Was.Connector.NettyServer.Codecs
{
    public class SmartServerDecoder : ByteToMessageDecoder
    {
        private static IByteBuffer HeaderBuffer = Unpooled.WrappedBuffer(new byte[] { 0xFF, 0xFF });
        protected override void Decode(IChannelHandlerContext context, IByteBuffer buffer, List<object> output)
        {
            //数据的最小长度为0x0E=14 不符合最小长度不解析 等待下一个包的到来
            if (buffer.ReadableBytes >= 14)
            {
                // 防止socket字节流攻击
                // 防止，客户端传来的数据过大
                // 因为，太大的数据，是不合理的
                if (buffer.ReadableBytes > 2048)
                {
                    buffer.SkipBytes(buffer.ReadableBytes);
                }

                var headerBeforeFrameLength = IndexOf(buffer, HeaderBuffer);
                if (headerBeforeFrameLength >= 0)
                {
                    buffer.SkipBytes(headerBeforeFrameLength);
                }
                else
                {
                    buffer.SkipBytes(buffer.ReadableBytes);
                }

                // 消息的长度
                int length = buffer.GetUnsignedShort(buffer.ReaderIndex + 2);
                // 判断请求数据包数据是否到齐 长度4为已经读取的起始位和长度
                if (buffer.ReadableBytes < length)
                {
                    return;
                }
                var bufferNew = buffer.ReadBytes(length);
                //输出一个起始位和长度都符合要求的包 此时如果长度后边有异常信息会继续解析一直到下一个报文头的位置 异常信息会被skip
                output.Add(bufferNew);
            }
            return;
        }


        /// <summary>
        /// Returns the number of bytes between the readerIndex of the haystack and
        /// the first needle found in the haystack.  -1 is returned if no needle is
        /// found in the haystack.
        /// </summary>
        static int IndexOf(IByteBuffer haystack, IByteBuffer needle)
        {
            for (int i = haystack.ReaderIndex; i < haystack.WriterIndex; i++)
            {
                int haystackIndex = i;
                int needleIndex;
                for (needleIndex = 0; needleIndex < needle.Capacity; needleIndex++)
                {
                    if (haystack.GetByte(haystackIndex) != needle.GetByte(needleIndex))
                    {
                        break;
                    }
                    else
                    {
                        haystackIndex++;
                        if (haystackIndex == haystack.WriterIndex && needleIndex != needle.Capacity - 1)
                        {
                            return -1;
                        }
                    }
                }

                if (needleIndex == needle.Capacity)
                {
                    // Found the needle from the haystack!
                    return i - haystack.ReaderIndex;
                }
            }
            return -1;
        }
    }
}
