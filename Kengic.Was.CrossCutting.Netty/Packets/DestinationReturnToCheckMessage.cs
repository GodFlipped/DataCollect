using DotNetty.Buffers;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCutting.Netty.Packets
{
    public class DestinationReturnToCheckMessage : NettyClientMessageBody
    {
        //解码需要 这里传输的是二进制字节 服务端接收
        public DestinationReturnToCheckMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            ScannerType = byteBuffer.ReadByte();
            ScannerNo = byteBuffer.ReadByte();
            MsgSequence = byteBuffer.ReadUnsignedInt();
            CarrierNo = byteBuffer.ReadUnsignedShort();
            BarLength = byteBuffer.ReadUnsignedShort();
            Barcode = byteBuffer.ReadString(BarLength, Encoding.ASCII);
        }

        //编码需要 这里把需要发送消息转换成二进制 服务端发送
        public DestinationReturnToCheckMessage(ushort messageType, byte scannerType, byte scannerNo, uint msgSequence, ushort carrierNo, ushort barLength, string barcode) : base(messageType)
        {
            MessageLength = (ushort)(16 + barLength);
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
