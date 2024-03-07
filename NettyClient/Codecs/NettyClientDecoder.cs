using DataCollect.Application.Helper;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Kengic.Was.Connector.NettyClient.Codecs
{
    public class NettyClientDecoder : ByteToMessageDecoder
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
                var functionCode = input.GetUnsignedShort(29);//29位以后是消息类型
                switch (functionCode)
                {
                    //心跳消息
                    case 1:
                        var heartBeatBody = new HeartBeatMessage(input);
                        heartBeatBody.DataContext = dataContext;
                        nettyClientMessage.nettyClientMessageBodies.Add(heartBeatBody);
                        break;
                    //起始消息
                    case 2:
                        var initBody = new InitMessage(input);
                        initBody.DataContext = dataContext;
                        nettyClientMessage.nettyClientMessageBodies.Add(initBody);
                        break;
                    //线体启停
                    case 400:
                        var startStopMessage = new StartStopMessage(input);
                        startStopMessage.DataContext = dataContext;
                        nettyClientMessage.nettyClientMessageBodies.Add(startStopMessage);
                        break;
                    //功能操作
                    case 421:
                        var functionOperationMessage = new FunctionOperationMessage(input);
                        functionOperationMessage.DataContext = dataContext;
                        nettyClientMessage.nettyClientMessageBodies.Add(functionOperationMessage);
                        break;
                    //设备参数设置
                    case 434:
                        var equipmentParameterSetting = new EquipmentParameterSetting(input);
                        equipmentParameterSetting.DataContext = dataContext;
                        nettyClientMessage.nettyClientMessageBodies.Add(equipmentParameterSetting);
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
                var nettyClientMessage = new NettyClientMessage(0xFFFF, 3000);
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
                    var nettyClientMessage = new NettyClientMessage(0xFFFF, 3000);
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
