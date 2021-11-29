namespace Tiveria.Home.Modbus
{
    public ref struct ReadRegistersResponse
    {
        public ushort StartingAddress { get; init; }
        public ushort Quantity { get; init; }
        public Span<byte> Payload { get; init; }
    }
}