using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 状态上报
    /// </summary>
    public class StateUpload:NettyClientMessageBody
    {
        public List<StateUploadPart> StateUploadList { get; set; }
        public StateUpload(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            var stateUploadPart = new StateUploadPart();
            StateUploadList = new List<StateUploadPart>();
            stateUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
            stateUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
            stateUploadPart.EquipmentState = byteBuffer.ReadUnsignedShort();
            StateUploadList.Add(stateUploadPart);
            if (MessageLength - 10 > 0)//如果总长度>13，肯定是有多个集合
            {
                for (var i = 0; i < (MessageLength - 10) / 6; i++)
                {
                    stateUploadPart.EquipmentType = byteBuffer.ReadUnsignedShort();
                    stateUploadPart.EquipmentNo = byteBuffer.ReadUnsignedShort();
                    stateUploadPart.EquipmentState = byteBuffer.ReadUnsignedShort();
                    StateUploadList.Add(stateUploadPart);
                }
            }
        }

        public StateUpload(ushort msgType, List<StateUploadPart> stateUploadList) : base(msgType)
        {
            StateUploadList = new List<StateUploadPart>();
            var stateUploadPart = new StateUploadPart();
            foreach (var item in stateUploadList)
            {
                stateUploadPart.EquipmentType = item.EquipmentType;
                stateUploadPart.EquipmentNo = item.EquipmentNo;
                stateUploadPart.EquipmentState = item.EquipmentState;
                StateUploadList.Add(stateUploadPart);
            }
            MessageLength = (ushort)(stateUploadList.Count * 6 + 4);
        }
        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            foreach (var item in StateUploadList)
            {
                byteBuffer.WriteUnsignedShort(item.EquipmentType);
                byteBuffer.WriteUnsignedShort(item.EquipmentNo);
                byteBuffer.WriteUnsignedShort(item.EquipmentState);
            }
            return byteBuffer;
        }
    }
    public class StateUploadPart
    {
        public ushort EquipmentType { get; set; }
        public ushort EquipmentNo { get; set; }
        public ushort EquipmentState { get; set; }
    }
}
