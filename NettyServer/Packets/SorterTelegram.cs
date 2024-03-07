using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetty.Buffers;
using Kengic.Was.Connector.NettyServer.Codecs;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    public class SorterTelegram : ICodec
    {
        public static ushort MaxLength = 1024;

        public SorterTelegram()
        {
            SorterTelegramHeader = new SorterTelegramHeader(this);
        }

        public SorterTelegramHeader SorterTelegramHeader { get; set; }
        public List<SorterMessage> SorterTelegramBodies { get; } = new List<SorterMessage>();

        public void Encode(IByteBuffer byteBuffer)
        {
            return;
        }

        public bool Decode(IByteBuffer byteBuffer, ref int remainingLength)
        {
            if (!byteBuffer.IsReadable(SorterTelegramHeader.ByteLength))
            {
                return false;
            }
            if (!SorterTelegramHeader.Decode(byteBuffer, ref remainingLength))
            {
                return false;
            }
            remainingLength = SorterTelegramHeader.Length - SorterTelegramHeader.ByteLength;
            if (!byteBuffer.IsReadable(remainingLength))
            {
                return false;
            }

            while (remainingLength > 0)
            {
                var messageId = byteBuffer.GetUnsignedShort(byteBuffer.ReaderIndex+2);
                SorterMessage sorterMessage;
                var originMessageLength = remainingLength;
                switch (messageId)
                {
                    case 01:
                        var statusBody = new HeartMessage();
                        if (!statusBody.Decode(byteBuffer, ref remainingLength))
                        {
                            return false;
                        }
                        sorterMessage = statusBody;
                        break;
                    default:
                        var currentAssemblyName = this.GetType().AssemblyQualifiedName;
                        var currentMethodName = this.GetType().FullName + "_" + MethodBase.GetCurrentMethod().Name + "_";
                        sorterMessage = null;
                        break;
                        //        throw new DecoderException(
                        //                                   $"First packet byte value of `{messageId}` is invalid.");
                }
                SorterTelegramBodies.Add(sorterMessage);
            }
            return true;
        }

        private void ValidMessage()
        {
            var bodySumByteLength = SorterTelegramHeader.ByteLength +
                                    SorterTelegramBodies.Sum(r => r.MessageLength);

            if (SorterTelegramHeader.TelegramLength != bodySumByteLength)
            {
                var currentAssemblyName = this.GetType().AssemblyQualifiedName;
                var currentMethodName = this.GetType().FullName + "_" + MethodBase.GetCurrentMethod().Name + "_";
                //throw new WasException(
                //                       $"Tim Header TelegramLength {SorterTelegramHeader.TelegramLength} Not Equal Sum of SorterMessage Bodys ByteLength  {bodySumByteLength}!");
            }
        }
    }
}