using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Generic;
using System.Net;

namespace Kengic.Was.Connector.NettyServer.Codecs
{
    public class NettyServerDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                var localAddr = context.Channel.LocalAddress;
                var ipEndPort = localAddr as IPEndPoint;
                var port = ipEndPort.Port;

                var messageLength = input.ReadableBytes;
                byte[] data = new byte[messageLength];
                input.GetBytes(0, data);
                var dataContext = BitConverter.ToString(data).Replace("-", "");

                NettyClientMessage nettyClientMessage = new NettyClientMessage(input, port);

                var functionCode = input.GetUnsignedShort(10);
                switch (functionCode)
                {
                    //心跳消息
                    case 1:
                        var heartBeatBody = new HeartBeatMessage(input);
                        heartBeatBody.DataContext = dataContext;
                        nettyClientMessage.nettyClientMessageBodies.Add(heartBeatBody);
                        break;
                    default:
                        var errorBody = new ErrorMessage(input, nettyClientMessage.Length);
                        nettyClientMessage.nettyClientMessageBodies.Add(errorBody);
                        break;
                }
                output.Add(nettyClientMessage);
            }
            catch (Exception)
            {
                //在解析内容出错的情况下输出整个消息内容
                input.SetReaderIndex(0);
                byte[] bytes = new byte[input.ReadableBytes];
                input.ReadBytes(bytes);
                var message = BitConverter.ToString(bytes).Replace("-", string.Empty);
                var nettyClientMessage = new NettyClientMessage(0xFFFF,2000);
                var errorMessage = new ErrorMessage(null, nettyClientMessage.Length)
                {
                    DataContext = message
                };
                nettyClientMessage.nettyClientMessageBodies.Add(errorMessage);
                output.Add(nettyClientMessage);
                return;
            }
            finally
            {
                if (input.ReadableBytes > 0)
                {
                    //在消息长度异常的情况下 输出剩余的尾部消息
                    byte[] bytes = new byte[input.ReadableBytes];
                    input.ReadBytes(bytes);
                    var message = BitConverter.ToString(bytes).Replace("-", string.Empty);
                    var nettyClientMessage = new NettyClientMessage(0xFFFF,2000);
                    var errorMessage = new ErrorMessage(null, nettyClientMessage.Length)
                    {
                        DataContext = message
                    };
                    nettyClientMessage.nettyClientMessageBodies.Add(errorMessage);
                    output.Add(nettyClientMessage);
                    //input.SkipBytes(input.ReadableBytes);
                }
            }
        }

    }
}
