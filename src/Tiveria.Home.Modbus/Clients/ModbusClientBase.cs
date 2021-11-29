using Tiveria.Common;
using Tiveria.Common.IO;

namespace Tiveria.Home.Modbus
{
    public abstract class ModbusClientBase : IDisposable
    {
        #region private fields
        protected byte[] _writeBuffer = new byte[260];
        protected byte[] _readBuffer = new byte[260];
        protected EndianBinaryWriter _writer;
        protected EndianBinaryReader _reader;
        private bool _disposedValue = false;
        #endregion

        #region public properties
        public Endian Endianess { get; init; }
        public byte ADUHeaderSize { get; init; } = 0;
        #endregion

        #region Constructors & Finalizer
        public ModbusClientBase(byte aduHeaderSize)
        {
            Endianess = Endian.Big;
            ADUHeaderSize = aduHeaderSize;
            _writer = new EndianBinaryWriter(new MemoryStream(_writeBuffer), Endianess);
            _reader = new EndianBinaryReader(new MemoryStream(_readBuffer), Endianess);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _reader.Dispose();
                    _writer.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region internal helpers

        protected void WriteGenericMultiReadRequestPDU(ushort startingAddress, ushort quantity, FunctionCodes functionCode)
        {
            _writer.Write((byte)functionCode);
            _writer.Write((ushort)startingAddress);
            _writer.Write((ushort)quantity);
        }

        protected void WriteGenericSingleWriteRequestPDU(ushort address, short registerdata, FunctionCodes functionCode)
        {
            _writer.Write((byte)functionCode);
            _writer.Write((ushort)address);
            _writer.Write((ushort)registerdata);
        }
        protected void WriteMultipleRegistersWriteRequestPDU(ushort startingAddress, short[] registerdata)
        {
            _writer.Write((byte)FunctionCodes.WriteMultipleRegisters);
            _writer.Write((ushort)startingAddress);
            _writer.Write((ushort)registerdata.Length);
            _writer.Write((byte)(registerdata.Length * 2));
            foreach(var register in registerdata)
                _writer.Write(register);
        }

        protected void WriteMultipleCoilsWriteRequestPDU(ushort startingAddress, bool[] coilsdata)
        {
            byte[] data = new byte[(int)Math.Round(coilsdata.Length / 8.0)];
            _writer.Write((byte)FunctionCodes.WriteMultipleCoils);
            _writer.Write((ushort)startingAddress);
            _writer.Write((ushort)coilsdata.Length);
            _writer.Write((byte)data.Length);

            for (int i = 0; i < coilsdata.Length; i++)
            {
                if (!coilsdata[i])
                    continue;
                var coilbit = i % 8;
                var coilbyte = i / 8;
                data[coilbyte] |= (byte) (1 << coilbit);
            }

            foreach (var bt in data)
                _writer.Write(bt);
        }

        protected void WriteMaskWriteRegisterRequestPDU(ushort address, ushort andMask, ushort orMask)
        {
            _writer.Write((byte)FunctionCodes.MaskWriteRegister);
            _writer.Write((ushort)address);
            _writer.Write((ushort)andMask);
            _writer.Write((ushort)orMask);
        }

        protected void WriteReadWriteMultipleRegistersRequestPDU(ushort writeStartAddress, short[] registerdata, ushort readStartAddress, ushort readQuantity)
        {
            _writer.Write((byte)FunctionCodes.ReadWriteMultipleRegisters);
            _writer.Write((ushort)readStartAddress);
            _writer.Write((ushort)readQuantity);
            _writer.Write((ushort)writeStartAddress);
            _writer.Write((ushort)registerdata.Length);
            _writer.Write((byte)(registerdata.Length * 2));
            foreach (var register in registerdata)
                _writer.Write(register);
        }

        #endregion
    }
}