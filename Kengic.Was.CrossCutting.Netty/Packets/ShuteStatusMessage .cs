using DotNetty.Buffers;

namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    /// <summary>
    /// 目的地命令消息
    /// </summary>
    public class ShuteStatusMessage : NettyClientMessageBody
    {
        public ShuteStatusMessage(IByteBuffer byteBuffer) : base(byteBuffer)
        {
            Shute = byteBuffer.ReadUnsignedInt();
            ShuteStatus = byteBuffer.ReadUnsignedInt();
        }

        public ShuteStatusMessage(ushort messageType, uint shute, uint shuteStatus) : base(messageType)
        {
            MessageLength = (ushort)(8);
            Shute = shute;
            ShuteStatus = shuteStatus;
        }
        
        public uint Shute { get; set; }
        public uint ShuteStatus { get; set; }
        

        public override IByteBuffer GetByteBuffer()
        {
            var byteBuffer = Unpooled.Buffer();
            byteBuffer.WriteUnsignedShort(MessageLength);
            byteBuffer.WriteUnsignedShort(MessageType);
            byteBuffer.WriteInt((int)Shute);
            byteBuffer.WriteInt((int)ShuteStatus);
            return byteBuffer;
        }
    }
}
