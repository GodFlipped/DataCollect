using System.Linq;
using DotNetty.Buffers;
using Kengic.Was.Connector.NettyServer.Codecs;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    public class SorterTelegramHeader : ICodec
    {
        private readonly SorterTelegram _sorterMessage;
        private ushort? _telegramLength;

        public SorterTelegramHeader(SorterTelegram sorterMessage)
        {
            _sorterMessage = sorterMessage;
        }

        public int ByteLength { get; set; } = 8;

        public short StartChar { get; set; } = 0XFF;

        public ushort TelegramLength
        {
            get
            {
                if (_telegramLength.HasValue)
                {
                    return _telegramLength.Value;
                }
                return
                    (ushort)
                    (_sorterMessage.SorterTelegramHeader.ByteLength +
                     _sorterMessage.SorterTelegramBodies.Sum(r => r.MessageLength));
            }
            set { _telegramLength = value; }
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

        public void Encode(IByteBuffer byteBuffer)
        {
            byteBuffer.WriteShort(StartChar);
            byteBuffer.WriteUnsignedShort(TelegramLength);
        }

        public bool Decode(IByteBuffer byteBuffer, ref int remainingLength)
        {
            Start = byteBuffer.ReadUnsignedShort();
            Length = byteBuffer.ReadUnsignedShort();
            Sequence = byteBuffer.ReadUnsignedShort();
            Version = byteBuffer.ReadUnsignedShort();
            return true;
        }
    }
}