using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCutting.Common;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Kengic.Was.Connector.NettyServer
{
    public class NioServerHandler : SimpleChannelInboundHandler<NettyClientMessage>
    {
        public string LogName { get; set; }
        public override bool IsSharable => true;
        public event EventHandler<MessageEventArgs<NettyClientMessage>> OnReceiveSorterMessageHandler;
        public IChannelHandlerContext channelHandlerContext;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            channelHandlerContext = context;
            AddChannnelMap(context);
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            RemoveChannnelMap(context);
            base.ChannelInactive(context);
        }

        private void RemoveChannnelMap(IChannelHandlerContext ctx)
        {
            foreach (var key in NettyServer.dictionary.Keys)
            {
                if (NettyServer.dictionary.ContainsKey(key))
                {
                    if (NettyServer.dictionary[key].Equals(ctx))
                    {
                        NettyServer.dictionary.TryRemove(key, out ctx);
                    }
                }
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, NettyClientMessage msg)
        {
            OnReceiveSorterMessageHandler(this, new MessageEventArgs<NettyClientMessage>(msg));
        }
        public void SendMessage(NettyClientMessage complementCodeMessage)
        {
            channelHandlerContext.WriteAndFlushAsync(complementCodeMessage);
        }
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            var idleStateEvent = evt as IdleStateEvent;
            //超过指定时间没有发送消息则发送心跳
            if (idleStateEvent.State == IdleState.WriterIdle)
            {
                var sequence = SequenceCreator.GetSequenceNo();
                var nettyClientMessage = new NettyClientMessage((ushort)sequence,2000);
                var sorterResultMessage = new HeartBeatMessage(1, 4);
                nettyClientMessage.nettyClientMessageBodies.Add(sorterResultMessage);
                var sendJson = JsonConvert.SerializeObject(nettyClientMessage);
                context.WriteAndFlushAsync(nettyClientMessage);
            }
            //超过指定时间没有收到client信息则断开连接
            if (idleStateEvent.State == IdleState.ReaderIdle)
            {

                context.Channel.CloseAsync();
                var dictionary = NettyServer.dictionary;
                if (NettyServer.dictionary.Values.Contains(context))
                {
                    var item = dictionary.Where(kvp => kvp.Value == context).FirstOrDefault();
                    dictionary.TryRemove(item.Key,out IChannelHandlerContext outMessage);
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }

        private void AddChannnelMap(IChannelHandlerContext ctx)
        {
            if (!NettyServer.dictionary.Values.Contains(ctx))
            {
                var sequence = SequenceCreator.GetIdBySequenceNo();
                NettyServer.dictionary.TryAdd(sequence,ctx);
            }
        }
    }
}
