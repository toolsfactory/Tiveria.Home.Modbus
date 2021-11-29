namespace Tiveria.Home.Modbus
{


    public static class ReadRegistersResponseExtensions
    {
        public static short GetRegister(this ReadRegistersResponse response, ushort address)
        {
            if (address < response.StartingAddress || address > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            short value = 0;

            if (BitConverter.IsLittleEndian)
                value = (short)((response.Payload[(address - response.StartingAddress) * 2] << 8) + response.Payload[(address - response.StartingAddress) * 2 + 1]);
            else
                value = (short)((response.Payload[(address - response.StartingAddress) * 2 +1] << 8) + response.Payload[(address - response.StartingAddress) * 2]);
            return value;
        }

        public static ushort GetRegisterUnsigned(this ReadRegistersResponse response, ushort address)
        {
            return (ushort)response.GetRegister(address);
        }

        public static int GetRegistersAsInt(this ReadRegistersResponse response, ushort address)
        {
            if (address < response.StartingAddress || address + 1 > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            return ((response.GetRegisterUnsigned((ushort)(address + 1)) << 16) + response.GetRegisterUnsigned((ushort)(address)));
        }

        public static uint GetRegistersAsUInt(this ReadRegistersResponse response, ushort address)
        {
            if (address < response.StartingAddress || address + 1 > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            return (uint)response.GetRegistersAsInt(address);
        }

        public static long GetRegistersAsLong(this ReadRegistersResponse response, ushort address)
        {
            if (address < response.StartingAddress || address + 3 > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            long ll = response.GetRegisterUnsigned((ushort)address);
            long lh = response.GetRegisterUnsigned((ushort)(address + 1));
            long hl = response.GetRegisterUnsigned((ushort)(address + 2));
            long hh = response.GetRegisterUnsigned((ushort)(address + 3));
            return (ll + (lh << 16) + (hl << 32));
        }

        public static ulong GetRegistersAsULong(this ReadRegistersResponse response, ushort address)
        {
            return (ulong) response.GetRegistersAsLong(address);
        }
        public static float GetRegistersAsFloat(this ReadRegistersResponse response, ushort address)
        {
            if (address < response.StartingAddress || address + 1 > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            var data = new byte[4];
            data[0] = response.Payload[(address - response.StartingAddress) * 2 + 1];
            data[1] = response.Payload[(address - response.StartingAddress) * 2 + 0];
            data[2] = response.Payload[(address - response.StartingAddress) * 2 + 3];
            data[3] = response.Payload[(address - response.StartingAddress) * 2 + 2];
            return BitConverter.ToSingle(data);
        }

        public static double GetRegistersAsDouble(this ReadRegistersResponse response, ushort address)
        {
            if (address < response.StartingAddress || address + 3 > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            var data = new byte[4];
            data[0] = response.Payload[(address - response.StartingAddress) * 2 + 1];
            data[1] = response.Payload[(address - response.StartingAddress) * 2 + 0];
            data[2] = response.Payload[(address - response.StartingAddress) * 2 + 3];
            data[3] = response.Payload[(address - response.StartingAddress) * 2 + 2];
            data[4] = response.Payload[(address - response.StartingAddress) * 2 + 5];
            data[5] = response.Payload[(address - response.StartingAddress) * 2 + 4];
            data[6] = response.Payload[(address - response.StartingAddress) * 2 + 7];
            data[7] = response.Payload[(address - response.StartingAddress) * 2 + 6];
            return BitConverter.ToDouble(data);
        }

        public static string GetRegistersAsString(this ReadRegistersResponse response, ushort address, int count)
        {
            if (address < response.StartingAddress || address + count - 1 > response.StartingAddress + response.Quantity)
                throw new ArgumentOutOfRangeException("address");

            var data = new byte[count * 2];

            for (var i = 0; i < count; i++)
            {
                data[i * 2] = response.Payload[((address - response.StartingAddress + i)* 2) + 1];
                data[(i * 2)+1] = response.Payload[((address - response.StartingAddress + i) * 2)];
            }
            return System.Text.Encoding.ASCII.GetString(data);
        }

    }
}