namespace Tiveria.Home.Modbus
{
    /// <summary>
    /// Defines the standard function codes supported by Modbus. Details of each code can be found in the official spec.
    /// https://modbus.org/docs/Modbus_Application_Protocol_V1_1b3.pdf
    /// </summary>
    public enum FunctionCodes : byte
    {
        // Data Access - Bitwise
        ReadDiscreteInputs = 0x02,        // Section 6.2
        ReadCoils = 0x01,                 // Section 6.1
        WriteSingleCoil = 0x05,           // Section 6.5
        WriteMultipleCoils = 0x0F,        // Section 6.11

        // Data Access - 16 bit Registers
        ReadInputRegister = 0x04,         // Section 6.4
        ReadHoldingRegisters = 0x03,      // Section 6.3
        WriteSingleRegister = 0x06,       // Section 6.6
        WriteMultipleRegisters = 0x10,    // Section 6.12
        ReadWriteMultipleRegisters = 0x17,// Section 6.17
        MaskWriteRegister = 0x16,         // Section 6.16
        ReadFIFOQueue = 0x18,             // Section 6.18

        // Data Access - File records
        ReadFileRecord = 0x20,            // Section 6.14
        WriteFileRecord = 0x21,           // Section 6.15

        ErrorFlag = 0x80                  // Or'ed with initially requested function code and returned by a server in case request could not processed
    }

}