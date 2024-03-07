using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class StartSorterMessage : NettyClientMessageBody
    {
        public StartSorterMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            Gear = byteBuffer.ReadUnsignedShort();
            Speed = byteBuffer.ReadUnsignedShort();
            PhycialSorter = byteBuffer.ReadUnsignedShort();
        }

        public StartSorterMessage(ushort messageType, ushort gear, ushort speed, ushort phycialSorter) : base(messageType)
        {
            MessageLength = (ushort)(6);
            Gear = gear;
            Speed = speed;
            PhycialSorter = phycialSorter;
        }
        
        //档位置
        public ushort Gear { get; set; }
        //速度置
        public ushort Speed { get; set; }
        //分拣机层级 1：第一层 2：第二层
        public ushort PhycialSorter { get; set; }
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteUnsignedShort(Speed);
            byteBuffer.WriteUnsignedShort(Gear);
            byteBuffer.WriteUnsignedShort(PhycialSorter);
            return byteBuffer;
        }
    }
}
