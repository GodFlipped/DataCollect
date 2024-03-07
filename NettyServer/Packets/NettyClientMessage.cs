using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    public class NettyClientMessage
    {
        public static int HeadData = 0xFFFF;
        /// <summary>
        /// 主消息 因为SmartDecoder是第一步解析 传输到后续Decoder的消息起始一定是0xFF 所以在收到的消息如果有超过正常的报文之外的消息 
        /// 在解析完成一个完整的报文以后 后续的会继续解析 此时如果读取前两位不是0xFF则直接将异常消息输出
        /// </summary>
        public NettyClientMessage(IByteBuffer byteBuffer)
        {

            try
            {
                Start = byteBuffer.ReadUnsignedShort();
                Length = byteBuffer.ReadUnsignedShort();
                Sequence = byteBuffer.ReadUnsignedShort();
                Version = byteBuffer.ReadUnsignedShort();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public NettyClientMessage(ushort sequnce)
        {
            Start = 0xffff;
            Sequence = sequnce;
            Version = 0x0000;
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
        public ushort Version { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }


        public ushort GetTotalLength()
        {
            return (ushort)(8 + nettyClientMessageBodies.Sum(r => r.MessageLength));
        }

        public IList<NettyClientMessageBody> nettyClientMessageBodies { get; } = new List<NettyClientMessageBody>();
    }
}
