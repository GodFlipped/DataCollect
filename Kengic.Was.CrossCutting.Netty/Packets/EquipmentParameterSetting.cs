using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    ///   设备参数设置
    /// </summary>
    public class EquipmentParameterSetting:NettyClientMessageBody
    {
        public List<EquipmentParameterSettingPart> EquipmentParameterSettingList { get; set; }
        public EquipmentParameterSetting(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var equipmentParameterSettingPart = new EquipmentParameterSettingPart();
            EquipmentParameterSettingList = new List<EquipmentParameterSettingPart>();
            equipmentParameterSettingPart.ParameterType = byteBuffer.ReadUnsignedShort();
            equipmentParameterSettingPart.ParameterValue = byteBuffer.ReadUnsignedShort();
            EquipmentParameterSettingList.Add(equipmentParameterSettingPart);
            if (MessageLength - 8 > 0)//如果总长度>8，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 8) / 4; i++)
                {
                    equipmentParameterSettingPart.ParameterType = byteBuffer.ReadUnsignedShort();
                    equipmentParameterSettingPart.ParameterValue = byteBuffer.ReadUnsignedShort();
                    EquipmentParameterSettingList.Add(equipmentParameterSettingPart);
                }
            }
        }

        public EquipmentParameterSetting(ushort msgType,List<EquipmentParameterSettingPart> equipmentParameterSettingList) : base(msgType)
        {
            EquipmentParameterSettingList = new List<EquipmentParameterSettingPart>();
            var equipmentParameterSettingPart = new EquipmentParameterSettingPart();
            foreach (var item in equipmentParameterSettingList)
            {
                equipmentParameterSettingPart.ParameterType = item.ParameterType;
                equipmentParameterSettingPart.ParameterValue = item.ParameterValue;
                EquipmentParameterSettingList.Add(equipmentParameterSettingPart);
            }
            MessageLength = (ushort)(equipmentParameterSettingList.Count * 4 + 4);
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in EquipmentParameterSettingList)
            {
                byteBuffer.WriteUnsignedShort(item.ParameterType);
                byteBuffer.WriteUnsignedShort(item.ParameterValue);
            }
            return byteBuffer;
        }
    }
    public class EquipmentParameterSettingPart
    {
        public ushort ParameterType { get; set; }
        public ushort ParameterValue { get; set; }
    }
}
