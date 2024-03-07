using DotNetty.Transport.Channels;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.ReceivePlcTask
{
   public class ReturnPlcData
    {

        public string Id { get; set; }
        public string Subscribe { get; set; }
        /// <summary>
        /// 订阅关键字
        /// </summary>
        public NettyClientMessage Data { get; set; }

        public ConcurrentDictionary<string, IChannelHandlerContext> dictionary { get; set; }
    }
}
