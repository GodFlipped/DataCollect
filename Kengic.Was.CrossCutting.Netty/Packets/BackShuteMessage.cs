using DotNetty.Buffers;
using System.Collections.Generic;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class BackShuteMessage : NettyClientMessageBody
    {
        public BackShuteMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            Shutes = new List<ushort> { };
            if (MessageLength - 4 > 0)
            {
                for (var i = 0; i < (MessageLength - 4) / 2; i++)
                {
                    var shute = byteBuffer.ReadUnsignedShort();
                    Shutes.Add(shute);
                }
            }
        }
        public BackShuteMessage(ushort messageType, List<ushort> shutes) : base(messageType)
        {
            MessageLength = (ushort)(4);
            Shutes = shutes;
        }
        
        public List<ushort> Shutes { get; set; }
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);

            if (MessageLength - 4 != 0)
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
