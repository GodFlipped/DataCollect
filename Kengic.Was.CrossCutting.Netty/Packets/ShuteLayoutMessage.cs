using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class ShuteLayoutMessage : NettyClientMessageBody
    {
        public ShuteLayoutMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            PhycialSorter = byteBuffer.ReadUnsignedShort();
        }
        public ShuteLayoutMessage(ushort messageType, ushort phycialSorter) : base(messageType)
        {
            MessageLength = (ushort)(6);
            PhycialSorter = phycialSorter;
        }
        
        public ushort PhycialSorter { get; set; }
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(PhycialSorter);
            return byteBuffer;
        }
    }
}
