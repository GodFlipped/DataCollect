using DataCollect.Application.Helper;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Kengic.Infrastructure.Protocol.Rds.Commons;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kengic.Was.Connector.NettyClient
{
    public class NioClientHandler : SimpleChannelInboundHandler<NettyClientMessage>
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private NettyClient client;
        private int total = 0;

        public event EventHandler<ExceptionEventArgs<Exception>> OnError;
        public event EventHandler OnHandlerRemoved;

        public NioClientHandler(NettyClient client)
        {
            this.client = client;
        }

        public string LogName { get; set; }
        public override bool IsSharable => true;
        public event EventHandler<MessageEventArgs<NettyClientMessage>> OnReceiveSorterMessageHandler;
        public IChannelHandlerContext channelHandlerContext;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            channelHandlerContext = context;
            total = 0;
            AddChannnelMap(context);
            base.ChannelActive(context);
        }
        //连接断开和异常的情况下重新连接
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            client.ConnectStatus = false;
            client.RecSendMsgStatus = false;

            Reconnect();
            foreach (var key in NettyClient.dictionary.Keys)
            {
                if (NettyClient.dictionary.ContainsKey(key))
                {
                    if (NettyClient.dictionary[key].Equals(context))
                    {
                        NettyClient.dictionary.TryRemove(key, out context);
                    }
                }
            }
            OnHandlerRemoved?.Invoke(this, EventArgs.Empty);
            base.ChannelInactive(context);
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            client.ConnectStatus = false;
            client.RecSendMsgStatus = false;

            Reconnect();
            foreach (var key in NettyClient.dictionary.Keys)
            {
                if (NettyClient.dictionary.ContainsKey(key))
                {
                    if (NettyClient.dictionary[key].Equals(context))
                    {
                        NettyClient.dictionary.TryRemove(key, out context);
                    }
                }
            }
            OnError?.Invoke(this, new ExceptionEventArgs<Exception>(exception));
            base.ExceptionCaught(context, exception);
        }
        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            Reconnect();
            base.HandlerRemoved(context);
        }



        protected override void ChannelRead0(IChannelHandlerContext ctx, NettyClientMessage msg)
        {
            OnReceiveSorterMessageHandler(this, new MessageEventArgs<NettyClientMessage>(msg));
        }
        public void SendMessage(NettyClientMessage complementCodeMessage)
        {
            channelHandlerContext.WriteAndFlushAsync(complementCodeMessage);
        }

        public async Task Reconnect()
        {
            Interlocked.Increment(ref total);
            if (total == 1)
            {
               
                    await client.ConnectToServer();
            }
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            var idleStateEvent = evt as IdleStateEvent;
            //超过指定时间没有发送消息则发送心跳
            if (idleStateEvent.State == IdleState.WriterIdle)
            {
                var sequence = SequenceCreator.GetSequenceNo();
                var dateTime = DateTime.Now;
                byte xor = 0;
                //中转场+SYS
                string lineNo = "755WF-SYS";
                var nettyClientMessage = new NettyClientMessage(sequence, xor,dateTime, lineNo, 3000);
                var sorterResultMessage = new HeartBeatMessage(1, 4);
                nettyClientMessage.nettyClientMessageBodies.Add(sorterResultMessage);
                var sendJson = JsonConvert.SerializeObject(nettyClientMessage);
                context.WriteAndFlushAsync(nettyClientMessage);
            }
            //超过指定时间没有收到client信息则断开连接
            if (idleStateEvent.State == IdleState.ReaderIdle)
            {
                var dictionary = NettyClient.dictionary;
                if (NettyClient.dictionary.Values.Contains(context))
                {
                    var item = dictionary.Where(kvp => kvp.Value == context).FirstOrDefault();
                    dictionary.TryRemove(item.Key, out IChannelHandlerContext outMessage);
                }

                client.DisConnect();

            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }

        private void AddChannnelMap(IChannelHandlerContext ctx)
        {
            if (!NettyClient.dictionary.Values.Contains(ctx))
            {
                var sequence = SequenceCreator.GetIdBySequenceNo();
                NettyClient.dictionary.TryAdd(sequence, ctx);
            }
        }
    }
}
