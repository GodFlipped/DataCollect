using DotNetty.Buffers;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCutting.Netty.Packets
{
    public class NettyCheckClientMessage
    {
        public static int HeadData = 0xFFFF;
        /// <summary>
        /// 主消息 因为SmartDecoder是第一步解析 传输到后续Decoder的消息起始一定是0xFF 所以在收到的消息如果有超过正常的报文之外的消息 
        /// 在解析完成一个完整的报文以后 后续的会继续解析 此时如果读取前两位不是0xFF则直接将异常消息输出
        /// </summary>
        public NettyCheckClientMessage(IByteBuffer byteBuffer, int port)
        {
            try
            {
                Start = byteBuffer.ReadUnsignedShort();
                Length = byteBuffer.ReadUnsignedShort();
                Sequence = byteBuffer.ReadUnsignedShort();
                Version = byteBuffer.ReadLong();
                Port = port;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public NettyCheckClientMessage(ushort sequnce, long time,int port)
        {
            Start = 0xffff;
            Sequence = sequnce;
            Version = time;
            Port = port;
        }

        /// <summary>
        /// 起始字符
        /// </summary>
        public ushort Start { get; set; }

        /// <summary>
        /// 总长度
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public ushort Sequence { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        public ulong time { get; set; }

        public ushort GetTotalLength()
        {
            return (ushort)(14 + nettyClientMessageBodies.Sum(r => r.MessageLength));
        }

        public IList<NettyClientMessageBody> nettyClientMessageBodies { get; } = new List<NettyClientMessageBody>();
    }
}
