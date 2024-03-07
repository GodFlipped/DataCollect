using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 功能操作消息
    /// </summary>
    public class FunctionOperationMessage :NettyClientMessageBody
    {
        /// <summary>
        /// //1：整线 2：单机
        /// </summary>
        public ushort OperationObjectType { get; set; }
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }
        /// <summary>
        /// 1、复位 2、阻塞屏蔽 3、休眠屏蔽 4、分拣模式切换
        /// </summary>
        public ushort OperationType { get; set; }
        /// <summary>
        /// 1、置位（分拣模式切换：人工模式） 2、复位（分拣模式切换：分拣模式）
        /// </summary>
        public byte OperationFlag { get; set; }
        public FunctionOperationMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            OperationObjectType = byteBuffer.ReadUnsignedShort();
            EquipmentType = byteBuffer.ReadUnsignedShort();
            EquipmentNo = byteBuffer.ReadUnsignedShort();
            OperationType = byteBuffer.ReadUnsignedShort();
            OperationFlag = byteBuffer.ReadByte();
        }

        public FunctionOperationMessage(ushort msgType, ushort operationObjectType, ushort equipmentType, ushort equipmentNo, ushort operationType, byte operationFlag) : base(msgType)
        {
            OperationObjectType = operationObjectType;
            EquipmentType = equipmentType;
            EquipmentNo = equipmentNo;
            OperationType = operationType;
            OperationFlag = operationFlag;
            MessageLength = 13;
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(OperationObjectType);
            byteBuffer.WriteUnsignedShort(EquipmentType);
            byteBuffer.WriteUnsignedShort(EquipmentNo);
            byteBuffer.WriteUnsignedShort(OperationType);
            byteBuffer.WriteByte(OperationFlag);
            return byteBuffer;
        }
    }
}
