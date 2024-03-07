

using DotNetty.Buffers;
using System.Text;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class SorterResultMessage: NettyClientMessageBody
    {

        public SorterResultMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ScannerType = byteBuffer.ReadByte();
            ScannerNo = byteBuffer.ReadByte();
            MsgSequence = byteBuffer.ReadUnsignedInt();
            CarrierNo = byteBuffer.ReadUnsignedShort();
            RequestDest = byteBuffer.ReadUnsignedShort();
            FinaltDest = byteBuffer.ReadUnsignedShort();
            SorterResult = byteBuffer.ReadUnsignedShort();
            Barcode = byteBuffer.ReadString(15, Encoding.ASCII);
        }
        public SorterResultMessage(ushort msgType, ushort scannerType, ushort scannerNo, uint msgSequence, ushort carrierNo,ushort requestDest,ushort  finaltDest,ushort sorterResult, string barcode): base(msgType)
        {
            //确定消息长度，不会有异常
            MessageLength = 33;
            ScannerType = scannerType;
            ScannerNo = scannerNo;
            MsgSequence = msgSequence;
            CarrierNo = carrierNo;
            RequestDest = requestDest;
            FinaltDest = finaltDest;
            SorterResult = sorterResult;
            Barcode = barcode;
        }

        public ushort ScannerType { get; set; }

        public ushort ScannerNo { get; set; }

        public uint MsgSequence { get; set; }

        public ushort CarrierNo { get; set; }

        public ushort RequestDest { get; set; }

        public ushort FinaltDest { get; set; }

        public ushort SorterResult { get; set; }

        public string Barcode { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteByte(ScannerType);
            byteBuffer.WriteByte(ScannerNo);
            byteBuffer.WriteInt((int)MsgSequence);
            byteBuffer.WriteUnsignedShort(CarrierNo);
            byteBuffer.WriteUnsignedShort(RequestDest);
            byteBuffer.WriteUnsignedShort(FinaltDest);
            byteBuffer.WriteUnsignedShort(SorterResult);
            byteBuffer.WriteString(Barcode, Encoding.ASCII);
            return byteBuffer;
        }
    }

}
