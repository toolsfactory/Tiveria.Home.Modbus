namespace Tiveria.Home.Modbus
{
    public ref struct ReadBitfieldResponse
    {
        public ushort StartingAddress { get; init; }
        public ushort Quantity { get; init; }
        public Span<byte> Payload { get; init; }


        public bool GetCoil(UInt16 coilAddress)
        {
            if (coilAddress < StartingAddress || coilAddress >= StartingAddress + Quantity)
                throw new ArgumentOutOfRangeException(nameof(coilAddress), $"Address must be in range of {StartingAddress} and {StartingAddress + Quantity - 1}");

            var coilbit = (coilAddress - StartingAddress) % 8;
            var coilbyte = (coilAddress - StartingAddress) / 8;

            var mask = (byte)(1 << coilbit);
            bool result = (Payload[coilbyte] & mask) == mask;
            return result;
        }

    }
}