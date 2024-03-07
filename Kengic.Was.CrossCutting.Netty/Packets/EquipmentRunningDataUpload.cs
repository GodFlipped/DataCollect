using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 设备运行时数据上报
    /// </summary>
    public class EquipmentRunningDataUpload:NettyClientMessageBody
    {
        public List<EquipmentRunningDataUploadPart> EquipmentRunningDataUploadPartList { get; set; }
        public EquipmentRunningDataUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var equipmentRunningDataUploadPart = new EquipmentRunningDataUploadPart();
            EquipmentRunningDataUploadPartList = new List<EquipmentRunningDataUploadPart>();

            equipmentRunningDataUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
            equipmentRunningDataUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
            equipmentRunningDataUploadPart.UploadDataType = byteBuffer.ReadUnsignedShort();
            equipmentRunningDataUploadPart.RunningData = byteBuffer.ReadUnsignedShort();
            EquipmentRunningDataUploadPartList.Add(equipmentRunningDataUploadPart);
            if (MessageLength - 12 > 0)//如果总长度>12，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 12) / 8; i++)
                {
                    equipmentRunningDataUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
                    equipmentRunningDataUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
                    equipmentRunningDataUploadPart.UploadDataType = byteBuffer.ReadUnsignedShort();
                    equipmentRunningDataUploadPart.RunningData = byteBuffer.ReadUnsignedShort();
                    EquipmentRunningDataUploadPartList.Add(equipmentRunningDataUploadPart);
                }
            }
        }

        public EquipmentRunningDataUpload(ushort msgType, List<EquipmentRunningDataUploadPart> equipmentRunningDataUploadPartList) : base(msgType)
        {
            EquipmentRunningDataUploadPartList = new List<EquipmentRunningDataUploadPart>();
            var equipmentRunningDataUploadPart = new EquipmentRunningDataUploadPart();
            foreach (var item in equipmentRunningDataUploadPartList)
            {
                equipmentRunningDataUploadPart.EquipmentType = item.EquipmentType;
                equipmentRunningDataUploadPart.EquipmentNo = item.EquipmentNo;
                equipmentRunningDataUploadPart.UploadDataType = item.UploadDataType;
                equipmentRunningDataUploadPart.RunningData = item.RunningData;
                EquipmentRunningDataUploadPartList.Add(equipmentRunningDataUploadPart);
            }
            MessageLength = (ushort)(equipmentRunningDataUploadPartList.Count * 4 + 4);
        }
      

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in EquipmentRunningDataUploadPartList)
            {
                byteBuffer.WriteUnsignedShort(item.EquipmentType);
                byteBuffer.WriteUnsignedShort(item.EquipmentNo);
                byteBuffer.WriteUnsignedShort(item.UploadDataType);
                byteBuffer.WriteUnsignedShort(item.RunningData);
            }
            return byteBuffer;
        }
    }    
    public class EquipmentRunningDataUploadPart
    {
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }
        public ushort UploadDataType { get; set; }
        public ushort RunningData { get; set; }
    }
}
