using DotNetty.Buffers;
using System.Collections.Generic;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class LockCarridMessage : NettyClientMessageBody
    {
        public LockCarridMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            OperateType = byteBuffer.ReadUnsignedShort();
            Carrids = new List<ushort> { };
            if (MessageLength - 6 > 0)
            {
                for (var i = 0; i < (MessageLength - 6) / 2; i++)
                {
                    var carrid = byteBuffer.ReadUnsignedShort();
                    Carrids.Add(carrid);
                }
            }

            //Carrids = byteBuffer.ReadString(MessageLength - 6, Encoding.ASCII);
            //Carrid = byteBuffer.ReadString(byteBuffer.ReadableBytes - 6, Encoding.ASCII);
        }

        public LockCarridMessage(ushort messageType, ushort operateType, List<ushort> carrids) : base(messageType)
        {
            MessageLength = (ushort)(6 + Carrids.Count);
            OperateType = operateType;
            Carrids = carrids;
        }
        
        public ushort OperateType { get; set; }
        public List<ushort> Carrids { get; set; }



        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(OperateType);

            if (MessageLength - 6 != 0)
            {
                for (var i = 0; i <= Carrids.Count - 1; i++)
                {
                    var carrid = Carrids[i];
                    byteBuffer.WriteUnsignedShort(carrid);
                }
            }

            return byteBuffer;
        }
    }
}
