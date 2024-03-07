
using DataCollect.Interface.ReceivePlcTask;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Kengic.Was.Connector.Common;
using Kengic.Was.Connector.NettyClient.Codecs;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Kengic.Was.Connector.NettyClient
{
    public class NettyClient : IConnector
    {
        public ConcurrentDictionary<string, object> ReceiveDictionary { get; set; }
        public ConcurrentDictionary<string, object> ReceiveDataDictionary { get; set; }
        public ConcurrentDictionary<string, object> ReceiveStatusDictionary { get; set; }


        public ConcurrentDictionary<string, Tuple<string, object>> ReSendDictionary { get; set; }

        public string Id { get; set; }
        public bool RecSendMsgStatus { get; set; }
        public bool ConnectStatus { get; set; }
        public bool AlarmActiveStatus { get; set; }
        public bool InitializeStatus { get; set; }
        public bool ProcessIsBeginForAuto { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        IChannel bootstrapChannel;
        Bootstrap bootstrap;
        MultithreadEventLoopGroup bossGroup;
        //public event AsyncEventHandler<ExceptionEventArgs<Exception>> OnError;
        //public event AsyncEventHandler<EventArgs> OnHandlerRemoved;

        public string ip = "127.0.0.1";
        public int port = 3000;
        public bool Initialize()
        {
            ReceiveDictionary = new ConcurrentDictionary<string, object>();
            ReceiveDataDictionary = new ConcurrentDictionary<string, object>();
            ReceiveStatusDictionary = new ConcurrentDictionary<string, object>();
            ReSendDictionary = new ConcurrentDictionary<string, Tuple<string, object>>();
            InitializeStatus = true;
            //OnError += SorterClientOnError;
            //OnHandlerRemoved += SorterClientOnHandlerRemoved;
            Connect();
            return true;
        }

        //private Task SorterClientOnError(object source, ExceptionEventArgs<Exception> e)
        //{
        //    return Task.CompletedTask;
        //}
        //private Task SorterClientOnHandlerRemoved(object source, EventArgs e)
        //{
        //    return Task.CompletedTask;
        //}



        public static ConcurrentDictionary<string, IChannelHandlerContext> dictionary = new ConcurrentDictionary<string, IChannelHandlerContext>();

        public NettyClient()
        {
        }

        public void NioClientHandler_OnReceiveSorterMessageHandler(object sender, MessageEventArgs<NettyClientMessage> e)
        {
            var receiveMsg = e.Message;
            var receiveBodies = receiveMsg.nettyClientMessageBodies;
            ReturnPlcData returnData = new ReturnPlcData();
            returnData.Id = receiveMsg.Sequence.ToString();
            returnData.Data = receiveMsg;
            returnData.dictionary = dictionary;
            ReceivePlcMessageEvent.CreateInstance().PlcData = returnData;
          
        }

        public bool Connect()
        {
            var nioClientHandler = new NioClientHandler(this);

            nioClientHandler.OnReceiveSorterMessageHandler += NioClientHandler_OnReceiveSorterMessageHandler;

              bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();

            try
            {
                bootstrap = new Bootstrap();
                bootstrap
                    .Group(bossGroup)
                        .Channel<TcpSocketChannel>()
                        .Option(ChannelOption.TcpNodelay, true)
                        .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                        {
                            IChannelPipeline pipeline = channel.Pipeline;
                            pipeline.AddLast(new LoggingHandler("CONN"));
                            pipeline.AddLast(new IdleStateHandler(10, 4, 0));
                            pipeline.AddLast(new SmartClientDecoder());
                            pipeline.AddLast(new NettyClientCodec());
                            pipeline.AddLast(nioClientHandler);
                        }));
               
                var host = IPAddress.Parse(ip);
               

                bootstrap.RemoteAddress(new IPEndPoint(host, port));

                RunClientAsync().Wait();


                return true;
            }
            catch (Exception  ex)
            {
                ConnectStatus = false;
                RecSendMsgStatus = false;
                return false;
            }
        }


        private async Task RunClientAsync()
        {
            await ConnectToServer();
        }

        public async Task ConnectToServer()
        {
            try
            {
                var host = IPAddress.Parse(ip);
                var abcddd = bootstrap;
                bootstrapChannel = await bootstrap.ConnectAsync();
                ConnectStatus = true;
                RecSendMsgStatus = true;
            }
            catch (Exception)
            {
                ConnectStatus = false;
                RecSendMsgStatus = false;
                Thread.Sleep(5000);
                await ConnectToServer();
            }

        }

        public bool DisConnect()
        {
            ConnectStatus = false;
            RecSendMsgStatus = false;
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
                //var localAddr = channelHandlerContext.Channel.LocalAddress;
                //var ipEndPort = localAddr as IPEndPoint;
                //var port = ipEndPort.Port;
                //if (port == 2000)
                //{
                //    channelHandlerContext.WriteAndFlushAsync(message).Wait();
                //}
                var sendJson = JsonConvert.SerializeObject(message);
                channelHandlerContext.WriteAndFlushAsync(message);
            }
            return true;
        }

        public bool SendMessage(string ipMessage , object message)
        {
            if (dictionary.Count < 1)
            {
                return false;
            }
            var ipArrary = ipMessage.Split('|');
            IPAddress ipAddress = IPAddress.Parse("::ffff:" + ipArrary[0]);
            var connectEndPoint = new IPEndPoint(ipAddress, int.Parse(ipArrary[1]));
            var currentConnect = dictionary.Values.Where(r => r.Channel.RemoteAddress.ToString() == connectEndPoint.ToString()).ToList();

            //if (currentConnect.Count < 1)
            //{
            //    var reSendMessage = new Tuple<string, object>(ipMessage, message);
            //    ReSendDictionary.TryAdd(Guid.NewGuid().ToString("N"), reSendMessage);
            //}


            var channelHandlerContext = currentConnect.FirstOrDefault();
            var sendJson = JsonConvert.SerializeObject(message);
            try
            {
                channelHandlerContext.WriteAndFlushAsync(message);
            }
            catch (Exception e)
            {
            }
            return true;
        }

    }
}
