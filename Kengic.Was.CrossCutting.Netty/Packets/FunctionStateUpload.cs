using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 功能状态上报
    /// </summary>
    public class FunctionStateUpload :NettyClientMessageBody
    {
        public List<FuncionStateUploadPart> FunctionStateUploadList { get; set; }
        public FunctionStateUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var funcionStateUploadPart = new FuncionStateUploadPart();
            FunctionStateUploadList = new List<FuncionStateUploadPart>();
            funcionStateUploadPart.OperationObjectType = byteBuffer.ReadUnsignedShort();
            funcionStateUploadPart.EquipmentType = byteBuffer.ReadByte();
            funcionStateUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
            funcionStateUploadPart.OpeartionType = byteBuffer.ReadUnsignedShort();
            funcionStateUploadPart.CurrentOperationFlag = byteBuffer.ReadByte();
            FunctionStateUploadList.Add(funcionStateUploadPart);
            if (MessageLength - 13 > 0)//如果总长度>13，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 13) / 9; i++)
                {
                    funcionStateUploadPart.OperationObjectType = byteBuffer.ReadUnsignedShort();
                    funcionStateUploadPart.EquipmentType = byteBuffer.ReadByte();
                    funcionStateUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
                    funcionStateUploadPart.OpeartionType = byteBuffer.ReadUnsignedShort();
                    funcionStateUploadPart.CurrentOperationFlag = byteBuffer.ReadByte();
                    FunctionStateUploadList.Add(funcionStateUploadPart);
                }
            }
        }

        public FunctionStateUpload(ushort msgType, List<FuncionStateUploadPart> functionStateUploadList) : base(msgType)
        {
            FunctionStateUploadList = new List<FuncionStateUploadPart>();
            var funcionStateUploadPart = new FuncionStateUploadPart();
            foreach (var item in functionStateUploadList)
            {
                funcionStateUploadPart.OperationObjectType = item.OperationObjectType;
                funcionStateUploadPart.EquipmentType = item.EquipmentType;
                funcionStateUploadPart.EquipmentNo = item.EquipmentNo;
                funcionStateUploadPart.OpeartionType = item.OpeartionType;
                funcionStateUploadPart.CurrentOperationFlag = item.CurrentOperationFlag;
                FunctionStateUploadList.Add(funcionStateUploadPart);
            }
            MessageLength = (ushort)(functionStateUploadList.Count * 9 + 4);
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in FunctionStateUploadList)
            {
                byteBuffer.WriteUnsignedShort(item.OperationObjectType);
                byteBuffer.WriteUnsignedShort(item.EquipmentType);
                byteBuffer.WriteUnsignedShort(item.EquipmentNo);
                byteBuffer.WriteUnsignedShort(item.OpeartionType);
                byteBuffer.WriteByte(item.CurrentOperationFlag);
            }
            return byteBuffer;
        }
    }
    public class FuncionStateUploadPart
    {
        public ushort OperationObjectType { get; set; }
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }
        public ushort OpeartionType { get; set; }
        public byte CurrentOperationFlag { get; set; }
    }
}
