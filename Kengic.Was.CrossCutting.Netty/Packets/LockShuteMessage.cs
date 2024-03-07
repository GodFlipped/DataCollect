using DotNetty.Buffers;
using System.Collections.Generic;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 锁格消息
    /// </summary>
    public class LockShuteMessage : NettyClientMessageBody
    {
        public LockShuteMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            LockType = byteBuffer.ReadUnsignedShort();
            Shutes = new List<ushort> { };
            if (MessageLength - 6 > 0)
            {
                for (var i = 0; i < (MessageLength - 6) / 2; i++)
                {
                    var shute = byteBuffer.ReadUnsignedShort();
                    Shutes.Add(shute);
                }
            }
        }
        public LockShuteMessage(ushort messageType, ushort lockType, List<ushort> shutes) : base(messageType)
        {
            MessageLength = (ushort)(6+Shutes.Count);
            LockType = lockType;
            Shutes = shutes;
        }

        public ushort LockType { get; set; }
        
        public List<ushort> Shutes { get; set; }
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(LockType);

            if (MessageLength - 6 != 0)
            {
                for (var i = 0; i <= Shutes.Count - 1; i++)
                {
                    var shute = Shutes[i];
                    byteBuffer.WriteUnsignedShort(shute);
                }
            }
            return byteBuffer;
        }
    }
}
