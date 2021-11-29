namespace Tiveria.Home.Modbus
{
    public static class ModbusClientExtensions
    {
        public static int ReadHoldingRegistersAsShort(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 1).GetRegister(address);
        }

        public static int ReadHoldingRegistersAsUShort(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 1).GetRegisterUnsigned(address);
        }

        public static int ReadHoldingRegistersAsInt(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 2).GetRegistersAsInt(address);
        }

        public static uint ReadHoldingRegistersAsUInt(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 2).GetRegistersAsUInt(address);
        }

        public static long ReadHoldingRegistersAsLong(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 4).GetRegistersAsLong(address);
        }

        public static ulong ReadHoldingRegistersAsULong(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 4).GetRegistersAsULong(address);
        }

        public static float ReadHoldingRegistersAsFloat(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 2).GetRegistersAsFloat(address);
        }

        public static double ReadHoldingRegistersAsDouble(this IModbusClient client, ushort address)
        {
            return client.ReadHoldingRegisters(address, 4).GetRegistersAsDouble(address);
        }

        public static string ReadHoldingRegistersAsString(this IModbusClient client, ushort address, ushort count)
        {
            return client.ReadHoldingRegisters(address, count).GetRegistersAsString(address, count);
        }
    }
}