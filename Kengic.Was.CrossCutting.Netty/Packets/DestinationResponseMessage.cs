using DotNetty.Buffers;
using System.Text;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 补码结果消息
    /// </summary>
    public class DestinationResponseMessage : NettyClientMessageBody
    {
        public DestinationResponseMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ScannerType = byteBuffer.ReadByte();
            ScannerNo = byteBuffer.ReadByte();
            MsgSequence = byteBuffer.ReadUnsignedInt();
            Carrier = byteBuffer.ReadUnsignedShort();
            Destination = byteBuffer.ReadUnsignedShort();
            Barcode = byteBuffer.ReadString(15, Encoding.ASCII);
        }

        public DestinationResponseMessage(ushort msgType, byte scannerType, byte scannerNo, uint msgSequence, ushort carrierNo, ushort destination, string barcode) : base(msgType)
        {
            MessageLength = (ushort)(33);
            ScannerType = scannerType;
            ScannerNo = scannerNo;
            MsgSequence = msgSequence;
            Carrier = carrierNo;
            Destination = destination;
            Barcode = barcode;
        }

        public byte ScannerType { get; set; }

        public byte ScannerNo { get; set; }

        public uint MsgSequence { get; set; }

        public ushort Carrier { get; set; }

        public ushort Destination { get; set; }

        public string Barcode { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteByte(ScannerType);
            byteBuffer.WriteByte(ScannerNo);
            byteBuffer.WriteInt((int)MsgSequence);
            byteBuffer.WriteUnsignedShort(Carrier);
            byteBuffer.WriteUnsignedShort(Destination);
            byteBuffer.WriteString(Barcode, Encoding.ASCII);
            return byteBuffer;
        }
    }
}
