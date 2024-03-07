using DotNetty.Buffers;
using System.Collections.Generic;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MalfunctionMessage : NettyClientMessageBody
    {

        public MalfunctionMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            Statuses = new List<MalfunctionStatus> { };
            if (MessageLength - 4 > 0)
            {
                for (var i = 0; i < (MessageLength - 4) / 6; i++)
                {
                    var equipmentType= byteBuffer.ReadByte();
                    var errorIndex = byteBuffer.ReadByte();
                    var equipmentNum = byteBuffer.ReadUnsignedShort();
                    var faultType = byteBuffer.ReadUnsignedShort();
                    var malfunctionStatus = new MalfunctionStatus
                    {
                        EquipmentType = equipmentType,
                        ErrorIndex = errorIndex,
                        EquipmentNum = equipmentNum,
                        FaultType = faultType,
                    };
                    Statuses.Add(malfunctionStatus);
                }
            }
        }

        public MalfunctionMessage(ushort msgType, List<MalfunctionStatus> statuses) : base(msgType)
        {
            MessageLength = (ushort)(4+statuses.Count*6);
            Statuses = statuses;
        }

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            if (MessageLength - 4 != 0)
            {
                for (var i = 0; i <= Statuses.Count - 1; i++)
                {
                    var status = Statuses[i];
                    byteBuffer.WriteByte(status.EquipmentType);
                    byteBuffer.WriteByte(status.ErrorIndex);
                    byteBuffer.WriteShort(status.EquipmentNum);
                    byteBuffer.WriteShort(status.FaultType);
                }
            }
            return byteBuffer;
        }

        public List<MalfunctionStatus> Statuses { get; set; }

    }
}
