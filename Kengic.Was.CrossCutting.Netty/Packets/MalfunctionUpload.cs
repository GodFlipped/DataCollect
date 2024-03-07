using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 故障上报
    /// </summary>
    public class MalfunctionUpload:NettyClientMessageBody
    {
        public List<MalfunctionUploadPart> MalfunctionUploadList { get; set; }
        public MalfunctionUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var malfunctionUploadPart = new MalfunctionUploadPart();
            MalfunctionUploadList = new List<MalfunctionUploadPart>();
            malfunctionUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
            malfunctionUploadPart.ErrorNoIndex = byteBuffer.ReadByte();
            malfunctionUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
            malfunctionUploadPart.ErrorType = byteBuffer.ReadUnsignedShort();
            MalfunctionUploadList.Add(malfunctionUploadPart);
            if (MessageLength - 11 > 0)//如果总长度>11，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 11) / 7; i++)
                {
                    malfunctionUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
                    malfunctionUploadPart.ErrorNoIndex = byteBuffer.ReadByte();
                    malfunctionUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
                    malfunctionUploadPart.ErrorType = byteBuffer.ReadUnsignedShort();
                    MalfunctionUploadList.Add(malfunctionUploadPart);
                }
            }
        }

        public MalfunctionUpload(ushort msgType, List<MalfunctionUploadPart> malfunctionUploadList) : base(msgType)
        {
            MalfunctionUploadList = new List<MalfunctionUploadPart>();
            var malfunctionUploadPart = new MalfunctionUploadPart();
            foreach (var item in malfunctionUploadList)
            {
                malfunctionUploadPart.EquipmentType = item.EquipmentType;
                malfunctionUploadPart.ErrorNoIndex = item.ErrorNoIndex;
                malfunctionUploadPart.EquipmentNo = item.EquipmentNo;
                malfunctionUploadPart.ErrorType = item.ErrorType;
                MalfunctionUploadList.Add(malfunctionUploadPart);
            }
            MessageLength = (ushort)(malfunctionUploadList.Count * 7 + 4);
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in MalfunctionUploadList)
            {
                byteBuffer.WriteUnsignedShort(item.EquipmentType);
                byteBuffer.WriteByte(item.ErrorNoIndex);
                byteBuffer.WriteUnsignedShort(item.EquipmentNo);
                byteBuffer.WriteUnsignedShort(item.ErrorType);
            }
            return byteBuffer;
        }
    }
    public class MalfunctionUploadPart
    {
        public ushort EquipmentType { get; set; }
        public byte ErrorNoIndex { get; set; }
        public ushort EquipmentNo{ get; set; }
        public ushort ErrorType { get; set; }
    }
}
