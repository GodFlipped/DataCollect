using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Kengic.Was.CrossCutting.Common;
using Kengic.Was.CrossCutting.Logging;
using Kengic.Was.CrossCutting.Netty.Packets;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace Kengic.Was.Connector.NettyCheckeServer
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
            //var localAddr = channelHandlerContext.Channel.LocalAddress;
            //var ipEndPort = localAddr as IPEndPoint;
            //var port = ipEndPort.Port;
           
            //    var lineNo = "531W-GA";
            //   if (port == 2003 || port == 2004)
            //   {
            //     lineNo = "531W-GB";
            //   }
            //    if (lineNo.Length < 10)
            //    {
            //        lineNo = lineNo.PadRight(10, ' ');
            //    };

            //    var company = "KJ";

            //    if (company.Length < 4)
            //    {
            //        company = company.PadRight(4, ' ');
            //    }
            //    var sequence = SequenceCreator.GetSequenceNo();
            //    var sendMessage = new NettyClientMessage((ushort)sequence, port);
            //    var body = new NesInitMessage(3, lineNo, company, 1);
            //    sendMessage.nettyClientMessageBodies.Add(body);
            //    context.WriteAndFlushAsync(sendMessage);
            

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
                LogRepository.WriteInfomationLog(LogName, "SendMessage", sendJson);
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
