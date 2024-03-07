using DotNetty.Buffers;
using Kengic.Was.CrossCuttings.Netty.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCutting.Netty.Packets
{
    public class NesInitMessage : NettyClientMessageBody
    {
        public NesInitMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            DateTime = byteBuffer.ReadLong();
        }

        public NesInitMessage(ushort msgType, string lineno, string companyNo, byte lineType) : base(msgType)
        {
            MessageLength = 26;
            LineNo = lineno;
            CompanyNo = companyNo;
            LineType = lineType;

        }



        public long DateTime { get; set; }
        public string LineNo { get; set; }
        public string CompanyNo { get; set; }
        public byte LineType { get; set; }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteString(LineNo, Encoding.ASCII);
            byteBuffer.WriteString(CompanyNo, Encoding.ASCII);
            byteBuffer.WriteByte(LineType);
            return byteBuffer;
        }
    }



}
