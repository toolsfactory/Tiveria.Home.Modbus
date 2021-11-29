namespace Tiveria.Home.Modbus
{

    public interface IModbusClient
    {
        ReadRegistersResponse ReadHoldingRegisters(ushort startingAddress, ushort quantity);
        ReadRegistersResponse ReadInputRegisters(ushort startingAddress, ushort quantity);

        void WriteMultipeRegisters(ushort startingAddress, short[] registerdata);
        void WriteSingleRegister(ushort address, short registerdata);
        
        ReadBitfieldResponse ReadCoils(ushort startingAddress, ushort quantity);
        void WriteSingleCoil(ushort address, bool coildata);

        void MaskWriteRegister(ushort address, ushort andMask, ushort orMask);

        //        ReadDiscreteInputsResponse ReadDiscreteInputs(ushort startingAddress, ushort quantity, byte unitIdentifier = 0x01);
    }

}