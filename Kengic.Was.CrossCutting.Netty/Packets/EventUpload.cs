using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 事件上报
    /// </summary>
    public class EventUpload:NettyClientMessageBody
    {
        public List<EventUploadPart> EventUploadList { get; set; }
        public EventUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var eventUploadPart = new EventUploadPart();
            EventUploadList = new List<EventUploadPart>();
            eventUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
            eventUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
            eventUploadPart.EventType = byteBuffer.ReadUnsignedShort();
            eventUploadPart.EventStartEndFlag = byteBuffer.ReadByte();
            EventUploadList.Add(eventUploadPart);

            if (MessageLength - 8 > 0)//如果总长度>8，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 8) / 4; i++)
                {
                    eventUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
                    eventUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
                    eventUploadPart.EventType = byteBuffer.ReadUnsignedShort();
                    eventUploadPart.EventStartEndFlag = byteBuffer.ReadByte();
                    EventUploadList.Add(eventUploadPart);
                }
            }
        }

        public EventUpload(ushort msgType, List<EventUploadPart> eventUploadList) : base(msgType)
        {
            EventUploadList = new List<EventUploadPart>();
            var eventUploadPart = new EventUploadPart();
            foreach (var item in eventUploadList)
            {
                eventUploadPart.EquipmentType = item.EquipmentType;
                eventUploadPart.EquipmentNo = item.EquipmentNo;
                eventUploadPart.EventType = item.EventType;
                eventUploadPart.EventStartEndFlag = item.EventStartEndFlag;
                EventUploadList.Add(eventUploadPart);
            }
            MessageLength = (ushort)(eventUploadList.Count * 7 + 4);
        }


        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in EventUploadList)
            {
                byteBuffer.WriteUnsignedShort(item.EquipmentType);
                byteBuffer.WriteUnsignedShort(item.EquipmentNo);
                byteBuffer.WriteUnsignedShort(item.EventType);
                byteBuffer.WriteByte(item.EventStartEndFlag);
            }
            return byteBuffer;
        }
    }
    public class EventUploadPart
    {
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }
        public ushort EventType { get; set; }
        public byte EventStartEndFlag { get; set; }

    }
}
