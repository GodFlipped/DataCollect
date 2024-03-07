using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Kengic.Was.Connector.Common;
using Kengic.Was.CrossCutting.ConfigurationSection.Connectors;
using Kengic.Was.CrossCutting.Logging;
using Kengic.Was.CrossCutting.MessageCodes;
using Newtonsoft.Json;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Logging;
using Kengic.Was.Connector.NettyCheckeServer.Codecs;
using Kengic.Was.CrossCuttings.Netty.Packets;
using DotNetty.Handlers.Timeout;
using System;

namespace Kengic.Was.Connector.NettyCheckeServer
{
    public class NettyServer : IConnector
    {
        private string _logName;
        public ConcurrentDictionary<string, object> ReceiveDictionary { get; set; }
        public ConcurrentDictionary<string, object> ReceiveDataDictionary { get; set; }
        public ConcurrentDictionary<string, object> ReceiveStatusDictionary { get; set; }
        public string Id { get; set; }
        public bool RecSendMsgStatus { get; set; }
        public ConnectorElement ConnectorElement { get; set; }
        public bool ConnectStatus { get; set; }
        public bool AlarmActiveStatus { get; set; }
        public bool InitializeStatus { get; set; }
        public bool ProcessIsBeginForAuto { get; set; }

        public bool Initialize()
        {
            ReceiveDictionary = new ConcurrentDictionary<string, object>();
            ReceiveDataDictionary = new ConcurrentDictionary<string, object>();
            ReceiveStatusDictionary = new ConcurrentDictionary<string, object>();

            _logName = ConnectorElement.LogName;
            LogRepository.WriteInfomationLog(_logName, StaticParameterForMessage.InitializeConnectSuccess, Id);
            InitializeStatus = true;
            return true;
        }

        public NettyServer(ConnectorElement connectorElement)
        {
            ConnectorElement = connectorElement;
        }

        IChannel bootstrapChannel;

        public static ConcurrentDictionary<string, IChannelHandlerContext> dictionary = new ConcurrentDictionary<string, IChannelHandlerContext>();

        public void NioServerHandler_OnReceiveSorterMessageHandler(object sender, MessageEventArgs<NettyClientMessage> e)
        {
            var receiveMsg = e.Message;
            var receiveBodies = receiveMsg.nettyClientMessageBodies;
            switch (receiveMsg.Port)
            {
                case 2000:
                    ReceiveDataDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
                    break;

                case 2001:
                    ReceiveStatusDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
                    break;
                case 2003:
                    ReceiveDataDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);
                    break;

                case 2004:
                    ReceiveStatusDictionary.TryAdd(receiveMsg.Sequence.ToString(), receiveMsg);

                    break;
                default:
                    break;
            }


            foreach (var bodyMessage in receiveBodies)
            {
                var bodyJson = JsonConvert.SerializeObject(bodyMessage);
                LogRepository.WriteInfomationLog(_logName, "ReceiveMessage", bodyJson);
                if (bodyMessage.MessageType == 0xFFFF)
                {
                    var dataContext = bodyMessage.DataContext;
                    LogRepository.WriteInfomationLog(_logName, "ReceiveErrorMessage", dataContext);
                }

            }
        }

        public bool Connect()
        {
            var nioServerHandler = new NioServerHandler { LogName=_logName};
            nioServerHandler.OnReceiveSorterMessageHandler += NioServerHandler_OnReceiveSorterMessageHandler;


            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup(5);

            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 5)
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

            bootstrapChannel = Task.Run(() => bootstrap.BindAsync(ConnectorElement.Connection.Remote.Port)).Result;
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
            bootstrapChannel.WriteAndFlushAsync(sendBuffer).Wait();
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
                        LogRepository.WriteInfomationLog(_logName, "SendMessage", sendJson);
                        channelHandlerContext.WriteAndFlushAsync(message);
  
                    }
                    catch (Exception e)
                    {
                        LogRepository.WriteInfomationLog(_logName, "Exception", e.ToString() + sendJson);
                    }
                    
                    
                }
                //channelHandlerContext.WriteAndFlushAsync(message).Wait();

            }
            return true;
        }

        public string SyncSendMessage(string requestMessage)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(string ipMessage,object message)
        {
            if (dictionary.Count < 1)
            {
                return false;
            }
            LogRepository.WriteInfomationLog(_logName, "Connect Count", dictionary.Count.ToString());
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
                    channelHandlerContext.WriteAndFlushAsync(message);
                    LogRepository.WriteInfomationLog(_logName, "SendMessage", sendJson);
                }
            }
            return true;
        }

        public bool SendMessage(string key, string ipMessage, object message)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(object message, IConnector connector)
        {
            throw new NotImplementedException();
        }
    }
}
