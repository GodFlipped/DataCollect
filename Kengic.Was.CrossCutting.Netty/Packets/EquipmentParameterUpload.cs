using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    ///   设备参数上报
    /// </summary>
    public class EquipmentParameterUpload:NettyClientMessageBody
    {

        public List<EquipmentParameterUploadPart> EquipmentParameterUploadList { get; set; }
        public EquipmentParameterUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var equipmentParameterUploadPart = new EquipmentParameterUploadPart();
            EquipmentParameterUploadList = new List<EquipmentParameterUploadPart>();
            equipmentParameterUploadPart.ParameterType = byteBuffer.ReadUnsignedShort();
            equipmentParameterUploadPart.ParameterValue = byteBuffer.ReadUnsignedShort();
            EquipmentParameterUploadList.Add(equipmentParameterUploadPart);
            if (MessageLength - 8 > 0)//如果总长度>8，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 8) / 4; i++)
                {
                    equipmentParameterUploadPart.ParameterType = byteBuffer.ReadUnsignedShort();
                    equipmentParameterUploadPart.ParameterValue = byteBuffer.ReadUnsignedShort();
                    EquipmentParameterUploadList.Add(equipmentParameterUploadPart);
                }
            }
        }

        public EquipmentParameterUpload(ushort msgType, List<EquipmentParameterUploadPart> equipmentParameterUploadList) : base(msgType)
        {
            EquipmentParameterUploadList = new List<EquipmentParameterUploadPart>();
            var equipmentParameterUploadPart = new EquipmentParameterUploadPart();
            foreach (var item in equipmentParameterUploadList)
            {
                equipmentParameterUploadPart.ParameterType = item.ParameterType;
                equipmentParameterUploadPart.ParameterValue = item.ParameterValue;
                EquipmentParameterUploadList.Add(equipmentParameterUploadPart);
            }
            MessageLength = (ushort)(equipmentParameterUploadList.Count * 4 + 4);
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in EquipmentParameterUploadList)
            {
                byteBuffer.WriteUnsignedShort(item.ParameterType);
                byteBuffer.WriteUnsignedShort(item.ParameterValue);
            }
            return byteBuffer;
        }
    }
    public class EquipmentParameterUploadPart
    {
        public ushort ParameterType { get; set; }
        public ushort ParameterValue { get; set; }
    }
}
