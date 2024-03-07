using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 启动/ 停止响应
    /// </summary>
    public class StartOrStopResponseMessage:NettyClientMessageBody
    {
        public StartOrStopResponseMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            StartType = byteBuffer.ReadUnsignedShort();
            StartOrStopState = byteBuffer.ReadByte();
            EquipmentType = byteBuffer.ReadUnsignedShort();
            EquipmentNo = byteBuffer.ReadUnsignedShort();
        }

        public StartOrStopResponseMessage(ushort msgType, ushort startType, byte startOrStopState, ushort equipmentType, ushort equipmentNo) : base(msgType)
        {
            StartType = startType;
            StartOrStopState = startOrStopState;
            EquipmentType = equipmentType;
            EquipmentNo = equipmentNo;
            MessageLength = 11;
        }
        public ushort StartType { get; set; }
        public byte StartOrStopState { get; set; }
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(StartType);
            byteBuffer.WriteByte(StartOrStopState);
            byteBuffer.WriteUnsignedShort(EquipmentType);
            byteBuffer.WriteUnsignedShort(EquipmentNo);
            return byteBuffer;
        }
    }
}
