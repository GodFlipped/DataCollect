using DotNetty.Buffers;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCutting.Netty.Packets
{
  public  class CheckRetrun : NettyClientMessageBody
    {
        public CheckRetrun(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ScannerType = byteBuffer.ReadByte();
            ScannerNo = byteBuffer.ReadByte();
            MsgSequence = byteBuffer.ReadUnsignedInt();
            CarrierNo = byteBuffer.ReadUnsignedShort();
            BarLength = byteBuffer.ReadUnsignedShort();
            Barcode = byteBuffer.ReadString(BarLength, Encoding.ASCII);
            Length = byteBuffer.ReadUnsignedShort();
            Wide = byteBuffer.ReadUnsignedShort();
            Height = byteBuffer.ReadUnsignedShort();
            Weight = byteBuffer.ReadUnsignedInt();
        }

        //编码需要 这里把需要发送消息转换成二进制 服务端发送
        public CheckRetrun(ushort messageType, byte scannerType, byte scannerNo, uint msgSequence, ushort carrierNo, ushort barLength, string barcode) : base(messageType)
        {
            MessageLength = (ushort)(14 + barLength);
            ScannerType = scannerType;
            ScannerNo = scannerNo;
            MsgSequence = msgSequence;
            CarrierNo = carrierNo;
            BarLength = barLength;
            Barcode = barcode;
        }


        public byte ScannerType { get; set; }

        public byte ScannerNo { get; set; }

        public uint MsgSequence { get; set; }

        public ushort CarrierNo { get; set; }

        public ushort BarLength { get; set; }

        public string Barcode { get; set; }

        public ushort Length { get; set; }

        public ushort Wide { get; set; }

        public ushort Height { get; set; }

        public uint Weight { get; set; }

        public string EndFied { get; set; }



        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteByte(ScannerType);
            byteBuffer.WriteByte(ScannerNo);
            byteBuffer.WriteInt((int)MsgSequence);
            byteBuffer.WriteUnsignedShort(CarrierNo);
            byteBuffer.WriteUnsignedShort(BarLength);
            byteBuffer.WriteString(Barcode, Encoding.ASCII);
            return byteBuffer;
        }
    }
}
