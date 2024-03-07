using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Generic;
using System.Net;

namespace Kengic.Was.Connector.NettyCheckeServer.Codecs
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
                NettyClientMessage nettyClientMessage = new NettyClientMessage(input, port);

                var functionCode = input.GetUnsignedShort(10);
                switch (functionCode)
                {
                    //起始状态消息
                    case 0:
                        var startStatusBody0 = new StartStatusMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(startStatusBody0);
                        break;
                    //心跳消息
                    case 1:
                        var heartBeatBody = new HeartBeatMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(heartBeatBody);
                        break;
                    //新起始状态消息
                    case 2:
                        var startStatusBody = new NewStartStatusMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(startStatusBody);
                        break;
                    case 3:
                        var startResponseBody = new StartResponseMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(startResponseBody);
                        break;
                    //确认消息
                    case 10:
                        var confirmBody = new ConfirmMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(confirmBody);
                        break;
                    //快件清空消息
                    case 20:
                        var clearBody = new JXDMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(clearBody);
                        break;
                    //复位消息
                    case 21:
                        var resetBody = new JXDResetMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(resetBody);
                        break;
                    //目的地
                    case 200:

                        var destinationResponseBody = new DestinationResponseMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(destinationResponseBody);
                        break;
                    //格口布局消息    --长度不定
                    case 201:
                        var shuteLayoutBody = new ShuteLayoutMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(shuteLayoutBody);
                        break;
                    //封锁格口消息
                    case 211:
                        var shuteCloseBody = new LockShuteMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(shuteCloseBody);
                        break;
                    //解锁格口消息
                    case 212:
                        var shuteOpenBody = new LockShuteMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(shuteOpenBody);
                        break;
                    //回流格口设置
                    case 213:
                        var backShuteBody = new BackShuteMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(backShuteBody);
                        break;
                    //格口状态消息
                    case 220:
                        var shuteStatusBody = new ShuteMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(shuteStatusBody);
                        break;
                    //打印完成消息
                    case 230:
                        var printerFinishBody = new ShuteMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(printerFinishBody);
                        break;
                    //JXD自检状态查询消息
                    case 240:
                        //var JXDSelfTestBody = new JXDMessage(input);
                        var JXDSelfTestBody = new MZDCheckStatus(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(JXDSelfTestBody);
                        break;
                    //格口显示消息
                    case 260:
                        var shuteDisplayBody = new ShuteDisplayMessage(input);
                        shuteDisplayBody.Sequence = input.GetUnsignedShort(4);
                        nettyClientMessage.nettyClientMessageBodies.Add(shuteDisplayBody);
                        break;
                    //封锁/解锁/查询小车消息    --长度不定
                    case 280:
                        var carridBody = new LockCarridMessage(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(carridBody);
                        break;
                    //启动
                    case 400:
                        var startSorterBody = new MZDRuningStatus(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(startSorterBody);
                        break;
                    case 292:
                        var modelChangeBody = new MZDRuningStatus(input);
                        nettyClientMessage.nettyClientMessageBodies.Add(modelChangeBody);
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
