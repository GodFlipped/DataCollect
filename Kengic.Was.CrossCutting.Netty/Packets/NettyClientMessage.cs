using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class NettyClientMessage
    {
        public static int HeadData = 0xFFFF;
        /// <summary>
        /// 主消息 因为SmartDecoder是第一步解析 传输到后续Decoder的消息起始一定是0xFF 所以在收到的消息如果有超过正常的报文之外的消息 
        /// 在解析完成一个完整的报文以后 后续的会继续解析 此时如果读取前两位不是0xFF则直接将异常消息输出
        /// </summary>
        public NettyClientMessage(IByteBuffer byteBuffer,int port)
        {

            try
            {
                Start = byteBuffer.ReadUnsignedShort();
                Length = byteBuffer.ReadUnsignedShort();
                Sequence = byteBuffer.ReadInt();
                XOR = byteBuffer.ReadByte();
                TimeStamp = byteBuffer.ReadLong();
                LineNo = byteBuffer.ReadString(10,Encoding.ASCII);
                Port = port;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public NettyClientMessage(int sequnce ,byte xor,DateTime timeStamp,string lineNo, int port)
        {
            Start = 0xffff;
            Sequence = sequnce;
            XOR = xor;
            TimeStamp = ((DateTimeOffset)timeStamp).ToUnixTimeMilliseconds();
            LineNo = lineNo.PadRight(10,' ');//不足10位后补空格
            Port = port;
        }
        public NettyClientMessage(int sequnce, int port)
        {
            Start = 0xffff;
            Sequence = sequnce;        
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
        public int Sequence { get; set; }

        /// <summary>
        /// 异或位
        /// </summary>
        public byte XOR { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// 线体编号
        /// </summary>
        public string LineNo { get;  set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }


        public ushort GetTotalLength()
        {
            //2+2+4+1+8+10
            return (ushort)(27 + nettyClientMessageBodies.Sum(r => r.MessageLength));
        }

        public IList<NettyClientMessageBody> nettyClientMessageBodies { get; } = new List<NettyClientMessageBody>();
    }
}
