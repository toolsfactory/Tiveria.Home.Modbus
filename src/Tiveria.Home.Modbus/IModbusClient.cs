namespace Tiveria.Home.Modbus
{

    public interface IModbusClient
    {
        ReadBitfieldResponse ReadCoils(ushort startingAddress, ushort quantity);
        ReadBitfieldResponse ReadDiscreteInputs(ushort startingAddress, ushort quantity);

        ReadRegistersResponse ReadHoldingRegisters(ushort startingAddress, ushort quantity);
        ReadRegistersResponse ReadInputRegisters(ushort startingAddress, ushort quantity);

        void WriteSingleCoil(ushort address, bool coildata);
        void WriteSingleRegister(ushort address, short registerdata);

        void WriteMultipleCoils(ushort address, bool[] coilsdata);
        void WriteMultipeRegisters(ushort startingAddress, short[] registerdata);
        void MaskWriteRegister(ushort address, ushort andMask, ushort orMask);

        ReadRegistersResponse ReadWriteMultipeRegisters(ushort writeStartAddress, short[] registerdata, ushort readStartAddress, ushort readQuantity);
    }
}