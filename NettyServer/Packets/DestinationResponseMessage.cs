using DotNetty.Buffers;
using System.Text;
using Kengic.Was.Connector.NettyClient.Packets;

namespace Kengic.Was.Connector.NettyServer.Packets
{
    /// <summary>
    /// 目的地命令消息
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

        public DestinationResponseMessage(ushort messageType, byte scannerType, byte scannerNo, uint messageSequence, ushort carrierNo, ushort destination, string barcode) : base(messageType)
        {
            MessageLength = (ushort)(29);
            ScannerType = scannerType;
            ScannerNo = scannerNo;
            MsgSequence = messageSequence;
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
