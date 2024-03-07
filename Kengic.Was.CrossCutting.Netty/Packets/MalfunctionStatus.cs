namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class MalfunctionStatus
    {
        public byte EquipmentType { get; set; }
        public byte ErrorIndex { get; set; }

        public ushort EquipmentNum { get; set; }
        public ushort FaultType { get; set; }
    }
}
