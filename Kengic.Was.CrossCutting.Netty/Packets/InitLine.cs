using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 初始化响应
    /// </summary>
    public class InitLine :NettyClientMessageBody
    {
        public InitLine(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            LineNoTotal = byteBuffer.ReadUnsignedShort();
            LineNo = new List<string> { };
            if (MessageLength - 20 > 0)
            {
                for (var i = 0; i < (MessageLength - 20) /16 ; i++)
                {
                    var lineno = byteBuffer.ReadString(10,Encoding.ASCII);
                    LineNo.Add(lineno);
                }
            }
            SupplierNo = byteBuffer.ReadString(4,Encoding.ASCII);
        }

        public InitLine(ushort msgType, ushort lineNoTotal, List<string> lineNo, string supplierNo) : base(msgType)
        {
            LineNoTotal = lineNoTotal;
            LineNo = lineNo;
            SupplierNo = supplierNo;
            MessageLength = (ushort)(4 + lineNo.Count * 10);
        }
        public ushort LineNoTotal { get; set; }
        public List<string> LineNo { get; set; }
        public string SupplierNo { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(LineNoTotal);
            foreach (var item in LineNo)
            {
                byteBuffer.WriteString(item,Encoding.ASCII);
            }
            byteBuffer.WriteString(SupplierNo,Encoding.ASCII);
            return byteBuffer;
        }
    }
}
