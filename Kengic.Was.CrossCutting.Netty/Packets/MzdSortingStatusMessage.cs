using DotNetty.Buffers;
using System.Collections.Generic;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MZDSortingStatusMessage : NettyClientMessageBody
    {

        public MZDSortingStatusMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            Statuses = new List<MZDSortingStatus> { };
            if (MessageLength - 4 > 0)
            {
                for (var i = 0; i < (MessageLength - 4) / 6; i++)
                {
                    var equipmentType= byteBuffer.ReadUnsignedShort();
                    var equipmentNum = byteBuffer.ReadUnsignedShort();
                    var status = byteBuffer.ReadUnsignedShort();
                    var malfunctionStatus = new MZDSortingStatus
                    {
                        EquipmentType = equipmentType,
                        EquipmentNum = equipmentNum,
                        Status = status,
                    };
                    Statuses.Add(malfunctionStatus);
                }
            }
        }

        public MZDSortingStatusMessage(ushort msgType, List<MZDSortingStatus> statuses) : base(msgType)
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
                    byteBuffer.WriteShort(status.EquipmentType);
                    byteBuffer.WriteShort(status.EquipmentNum);
                    byteBuffer.WriteShort(status.Status);
                }
            }
            return byteBuffer;
        }

        public List<MZDSortingStatus> Statuses { get; set; }

    }
}
