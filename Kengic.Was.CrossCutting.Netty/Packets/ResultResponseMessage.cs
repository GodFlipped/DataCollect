using DotNetty.Buffers;
using System.Text;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class ResultResponseMessage : NettyClientMessageBody
    {
        public ResultResponseMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ScannerType = byteBuffer.ReadByte();
            ScannerNo = byteBuffer.ReadByte();
            MsgSequence = byteBuffer.ReadUnsignedInt();
            Carrier = byteBuffer.ReadUnsignedShort();
            CurrentShuteAddr = byteBuffer.ReadUnsignedInt();
            FinalShuteAddr = byteBuffer.ReadUnsignedInt();
            SorterResult = byteBuffer.ReadUnsignedInt();
            Barcode = byteBuffer.ReadString(15, Encoding.ASCII);
            PhycialSorter = byteBuffer.ReadUnsignedInt();
        }

        public ResultResponseMessage(ushort messageType, byte scannerType, byte scannerNo, uint messageSequence, ushort carrierNo, uint currentShuteAddr, uint finalShuteAddr, uint sorterResult, string barcode, uint phycialSorter) : base(messageType)
        {
            MessageLength = (ushort)(33);
            ScannerType = scannerType;
            ScannerNo = scannerNo;
            MsgSequence = messageSequence;
            Carrier = carrierNo;
            CurrentShuteAddr = currentShuteAddr;
            FinalShuteAddr = finalShuteAddr;
            SorterResult = sorterResult;
            Barcode = barcode;
            PhycialSorter = phycialSorter;
        }

        public byte ScannerType { get; set; }

        public byte ScannerNo { get; set; }

        public uint MsgSequence { get; set; }

        public ushort Carrier { get; set; }

        public uint CurrentShuteAddr { get; set; }

        public uint FinalShuteAddr { get; set; }

        public uint SorterResult { get; set; } 

        public string Barcode { get; set; }

        public uint PhycialSorter { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteByte(ScannerType);
            byteBuffer.WriteByte(ScannerNo);
            byteBuffer.WriteInt((int)MsgSequence);
            byteBuffer.WriteUnsignedShort(Carrier);
            byteBuffer.WriteInt((int)CurrentShuteAddr);
            byteBuffer.WriteInt((int)FinalShuteAddr);
            byteBuffer.WriteInt((int)SorterResult);
            byteBuffer.WriteString(Barcode, Encoding.ASCII);
            byteBuffer.WriteInt((int)PhycialSorter);
            return byteBuffer;
        }
    }
}
