using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class EccVoltagePowerUpload:NettyClientMessageBody
    {
        /// <summary>
        /// 主电柜电压耗电量上报
        /// </summary>
        public List<EccVoltagePowerUploadPart> EccVoltagePowerUploadList { get; set; }
        public EccVoltagePowerUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var eccVoltagePowerUploadPart = new EccVoltagePowerUploadPart();
            EccVoltagePowerUploadList = new List<EccVoltagePowerUploadPart>();
            eccVoltagePowerUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.Power = byteBuffer.ReadInt();
            eccVoltagePowerUploadPart.AVoltage = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.AElectricity = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.BVoltage = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.BElectricity = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.CVoltage = byteBuffer.ReadUnsignedShort();
            eccVoltagePowerUploadPart.CElectricity = byteBuffer.ReadUnsignedShort();
            EccVoltagePowerUploadList.Add(eccVoltagePowerUploadPart);
            if (MessageLength - 24 > 0)//如果总长度>24，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 24) / 20; i++)
                {
                    eccVoltagePowerUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.Power = byteBuffer.ReadInt();
                    eccVoltagePowerUploadPart.AVoltage = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.AElectricity = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.BVoltage = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.BElectricity = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.CVoltage = byteBuffer.ReadUnsignedShort();
                    eccVoltagePowerUploadPart.CElectricity = byteBuffer.ReadUnsignedShort();
                    EccVoltagePowerUploadList.Add(eccVoltagePowerUploadPart);
                }
            }
        }

        public EccVoltagePowerUpload(ushort msgType,List<EccVoltagePowerUploadPart> eccVoltagePowerUploadList) : base(msgType)
        {
            EccVoltagePowerUploadList = new List<EccVoltagePowerUploadPart>();
            var eccVoltagePowerUploadPart = new EccVoltagePowerUploadPart();
            foreach (var item in eccVoltagePowerUploadList)
            {
                eccVoltagePowerUploadPart.EquipmentType = item.EquipmentType;
                eccVoltagePowerUploadPart.EquipmentNo = item.EquipmentNo;
                eccVoltagePowerUploadPart.Power = item.Power;
                eccVoltagePowerUploadPart.AVoltage = item.AVoltage;
                eccVoltagePowerUploadPart.AElectricity = item.AElectricity;
                eccVoltagePowerUploadPart.BVoltage = item.BVoltage;
                eccVoltagePowerUploadPart.BElectricity = item.BElectricity;
                eccVoltagePowerUploadPart.CVoltage = item.CVoltage;
                eccVoltagePowerUploadPart.CElectricity = item.CElectricity;
                EccVoltagePowerUploadList.Add(eccVoltagePowerUploadPart);
            }
            MessageLength =(ushort)(eccVoltagePowerUploadList.Count * 20 + 4);
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in EccVoltagePowerUploadList)
            {
                byteBuffer.WriteUnsignedShort(item.EquipmentType);
                byteBuffer.WriteUnsignedShort(item.EquipmentNo);
                byteBuffer.WriteInt(item.Power);
                byteBuffer.WriteUnsignedShort(item.AVoltage);
                byteBuffer.WriteUnsignedShort(item.AElectricity);
                byteBuffer.WriteUnsignedShort(item.BVoltage);
                byteBuffer.WriteUnsignedShort(item.BElectricity);
                byteBuffer.WriteUnsignedShort(item.CVoltage);
                byteBuffer.WriteUnsignedShort(item.CElectricity);
            }           
            return byteBuffer;
        }
    }
    public class EccVoltagePowerUploadPart
    {
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }
        public int Power { get; set; }
        public ushort AVoltage { get; set; }
        public ushort AElectricity { get; set; }
        public ushort BVoltage { get; set; }
        public ushort BElectricity { get; set; }
        public ushort CVoltage { get; set; }
        public ushort CElectricity { get; set; }
    }
}
