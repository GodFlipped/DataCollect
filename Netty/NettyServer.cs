
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Kengic.Was.Connector.NettyServer.Codecs;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Kengic.Was.Connector.NettyServer
{
    public class NettyServer
    {
        private string _logName;
        public ConcurrentDictionary<string, object> ReceiveDictionary { get; set; }
        public ConcurrentDictionary<string, object> ReceiveDataDictionary { get; set; }
        public ConcurrentDictionary<string, object> ReceiveStatusDictionary { get; set; }
        public string Id { get; set; }
        public bool RecSendMsgStatus { get; set; }
        public bool ConnectStatus { get; set; }
        public bool AlarmActiveStatus { get; set; }
        public bool InitializeStatus { get; set; }
        public bool ProcessIsBeginForAuto { get; set; }

        public bool Initialize()
        {
            ReceiveDictionary = new ConcurrentDictionary<string, object>();
            ReceiveDataDictionary = new ConcurrentDictionary<string, object>();
            ReceiveStatusDictionary = new ConcurrentDictionary<string, object>();
            InitializeStatus = true;
            return true;
        }


        IChannel bootstrapChannel;

        public static ConcurrentDictionary<string, IChannelHandlerContext> dictionary = new ConcurrentDictionary<string, IChannelHandlerContext>();

        public void NioServerHandler_OnReceiveSorterMessageHandler(object sender, MessageEventArgs<NettyClientMessage> e)
        {
            var receiveMsg = e.Message;
            var receiveBodies = receiveMsg.nettyClientMessageBodies;
            if (receiveBodies != null && receiveBodies[0].MessageType != 0x01 && receiveBodies[0].MessageType != 0xFFFF)
            {
                switch (2000)
                {
                    case 2000:
                        ReceiveDataDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
                        break;

                    case 2001:
                        var bodyMsg = receiveBodies[0];
                        if (bodyMsg.MessageType == 260)
                        {
                            ReceiveDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
                        }
                        else
                        {
                            ReceiveStatusDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
                        }
                        break;
                    default:
                        break;
                }

            }

            //ReceiveDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
            var sequence = receiveMsg.Sequence;

            foreach (var bodyMessage in receiveBodies)
            {
                var bodyJson = JsonConvert.SerializeObject(bodyMessage);
                var dataContext = bodyMessage.DataContext;
                if (bodyMessage.MessageType == 0xFFFF)
                {
                  
                }
 
            }
        }

        public bool Connect()
        {
            var nioServerHandler = new NioServerHandler { LogName = _logName };
            nioServerHandler.OnReceiveSorterMessageHandler += NioServerHandler_OnReceiveSorterMessageHandler;


            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();

            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .Handler(new LoggingHandler("LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new LoggingHandler("CONN"));
                    pipeline.AddLast(new IdleStateHandler(10, 4, 0));
                    pipeline.AddLast(new SmartServerDecoder());
                    pipeline.AddLast(new NettyServerCodec());
                    pipeline.AddLast(nioServerHandler);
                }));

            bootstrapChannel = Task.Run(() => bootstrap.BindAsync(8080)).Result;
            ConnectStatus = true;
            RecSendMsgStatus = true;
            return true;
        }

        public bool DisConnect()
        {
            bootstrapChannel.CloseAsync();
            return true;
        }

        public bool SendMessage(List<string> messageList)
        {
            if (!RecSendMsgStatus)
            {
                return false;
            }

            return (messageList.Count == 1) && SendMessage(messageList[0]);
        }

        public bool SendByte(ConcurrentQueue<byte[]> sendBuffer)
        {
            bootstrapChannel.WriteAndFlushAsync(sendBuffer);
            return true;
        }

        public bool SendMessage(object message)
        {
            if (dictionary.Count < 1)
            {
                return false;
            }
            foreach (var key in dictionary.Keys)
            {
                var channelHandlerContext = dictionary[key];
                var localAddr = channelHandlerContext.Channel.LocalAddress;
                var ipEndPort = localAddr as IPEndPoint;
                var port = ipEndPort.Port;
                if (port == 2000)
                {
                    var sendJson = JsonConvert.SerializeObject(message);
                    try
                    {
                        channelHandlerContext.WriteAndFlushAsync(message);
                    }
                    catch (Exception e)
                    {
                    }
                    
                    
                }
                //channelHandlerContext.WriteAndFlushAsync(message).Wait();

            }
            return true;
        }

    

        public bool SendMessage(string ipMessage,object message)
        {
            if (dictionary.Count < 1)
            {
                var sendJson = JsonConvert.SerializeObject(message);
                return false;
            }
            foreach (var key in dictionary.Keys)
            {

                var channelHandlerContext = dictionary[key];
                var localAddr = channelHandlerContext.Channel.LocalAddress;

                var ipArrary = ipMessage.Split('|');

                var ipEndPort = localAddr as IPEndPoint;
                var port = ipEndPort.Port;
                if (port == int.Parse(ipArrary[1]))
                {
                    var sendJson = JsonConvert.SerializeObject(message);
                    try
                    {
                        channelHandlerContext.WriteAndFlushAsync(message);
                    }
                    catch (Exception e)
                    {
                    }
                    
                }
            }
            return true;
        }

    }
}
