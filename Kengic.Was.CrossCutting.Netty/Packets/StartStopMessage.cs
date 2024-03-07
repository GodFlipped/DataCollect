using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 启动/ 停止
    /// </summary>
    public class StartStopMessage:NettyClientMessageBody
    {
        public StartStopMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            StartType = byteBuffer.ReadUnsignedShort();
            StartOrStop = byteBuffer.ReadByte();
            Gears = byteBuffer.ReadUnsignedShort();
            Speed = byteBuffer.ReadUnsignedShort();
            EquipmentType = byteBuffer.ReadUnsignedShort();
            EquipmentNo = byteBuffer.ReadUnsignedShort();
        }

        public StartStopMessage(ushort msgType, ushort startType, byte startOrStop, ushort gears, ushort speed, ushort equipmentType, ushort equipmentNo) : base(msgType)
        {
            StartType = startType;
            StartOrStop = startOrStop;
            Gears = gears;
            Speed = speed;
            EquipmentType = equipmentType;
            EquipmentNo = equipmentNo;
            MessageLength = 15;
        }
        public ushort StartType { get; set; }//1：整线 2：单机
        public byte StartOrStop { get; set; }//1：启动 2：停止 3：休眠 4：唤醒
        public ushort Gears { get; set; }  //档位 仅交叉带启动生效
        public ushort Speed { get; set; }//仅交叉带启动生效
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(StartType);
            byteBuffer.WriteByte(StartOrStop);
            byteBuffer.WriteUnsignedShort(Gears);
            byteBuffer.WriteUnsignedShort(Speed);
            byteBuffer.WriteUnsignedShort(EquipmentType);
            byteBuffer.WriteUnsignedShort(EquipmentNo);
            return byteBuffer;
        }
    }
}
