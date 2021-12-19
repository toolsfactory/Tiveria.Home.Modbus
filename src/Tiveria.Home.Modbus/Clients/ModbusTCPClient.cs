using System.Net;
using System.Net.Sockets;
using Tiveria.Common;
using Tiveria.Common.IO;
using Tiveria.Common.Logging;

namespace Tiveria.Home.Modbus
{
    public class ModbusTCPClient : ModbusClientBase, IModbusClient
    {
        #region private fields
        private ITcpConnection _tcpConnection;
        private bool disposedValue = false;
        private object _transactionIdLock = new object();
        private ushort _transactionId = 1;
        private ILogger? _logger;
        private long _requestSize = 0;
        private long _requestPayloadSize = 0;
        #endregion

        #region public properties
        public int ConnectTimeout { get; set; } = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;
        public bool Connected => _tcpConnection.Connected;
        public byte UnitIdentifier { get; init; }
        public ushort LastTransactionId => (ushort)(_transactionId - 1);
        #endregion

        #region Constructors & Finalizer
        public ModbusTCPClient(byte unitIdentifier = 1, ILogger? logger = null) 
            : base(7)
        {
            _tcpConnection = new TcpConnection();
            _logger = logger;
            UnitIdentifier = unitIdentifier;
        }

        public ModbusTCPClient(ITcpConnection tcpConnection, byte unitIdentifier = 1, ILogger? logger = null)
            : base(7)
        {
            _tcpConnection = tcpConnection ?? throw new ArgumentNullException(nameof(tcpConnection));
            _logger = logger;
            UnitIdentifier = unitIdentifier;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tcpConnection.Dispose();
                    _reader.Dispose();
                    _writer.Dispose();
                }
                disposedValue = true;
            }
        }
        #endregion

        #region Connecting & Disconnecting 
        public void Connect(IPEndPoint remoteEndpoint)
        {
            _tcpConnection.Connect(remoteEndpoint);
        }

        public void Connect(string hostname, int port = 502)
        {
            _tcpConnection.Connect(hostname, port);
        }

        public void Disconnect()
        {
            _tcpConnection.Close();
        }
        #endregion

        #region IModbusClient Methods implementation
        #region Reading registers
        public ReadRegistersResponse ReadHoldingRegisters(ushort startingAddress, ushort quantity)
            =>  GenericReadRegisters(startingAddress, quantity, FunctionCodes.ReadHoldingRegisters);
    
        public ReadRegistersResponse ReadInputRegisters(ushort startingAddress, ushort quantity)
            => GenericReadRegisters(startingAddress, quantity, FunctionCodes.ReadHoldingRegisters);
        #endregion

        #region Writing registers
        public void WriteSingleRegister(ushort address, short registerdata)
        => GenericWriteFunction(() => WriteGenericSingleWriteRequestPDU(address, registerdata, FunctionCodes.WriteSingleRegister));

        public void WriteMultipeRegisters(ushort startingAddress, short[] registerdata)
        {
            CheckQuantity(registerdata.Length, 0x7B); // 123
            GenericWriteFunction(
                () => WriteMultipleRegistersWriteRequestPDU(startingAddress, registerdata));
        }
        #endregion

        #region Reading bitfields
        public ReadBitfieldResponse ReadDiscreteInputs(ushort startingAddress, ushort quantity)
            => GenericReadBitField(startingAddress, quantity, FunctionCodes.ReadDiscreteInputs);

        public ReadBitfieldResponse ReadCoils(ushort startingAddress, ushort quantity)
            => GenericReadBitField(startingAddress, quantity, FunctionCodes.ReadCoils);
        #endregion

        #region Writing bitfields
        public void WriteSingleCoil(ushort address, bool coildata)
        {
            var registerdata = (short)(coildata ? 0xFF00 : 0x00);
            GenericWriteFunction(
                () => WriteGenericSingleWriteRequestPDU(address, registerdata, FunctionCodes.WriteSingleCoil));
        }

        public void WriteMultipleCoils(ushort address, bool[] coilsdata)
        {
            CheckQuantity(coilsdata.Length, 0x07B0); // 1968
            GenericWriteFunction(
                () => WriteMultipleCoilsWriteRequestPDU(address, coilsdata));
        }
        #endregion

        #region Advanced register operations
        public void MaskWriteRegister(ushort address, ushort andMask, ushort orMask)
            => GenericWriteFunction(() => WriteMaskWriteRegisterRequestPDU(address, andMask, orMask));

        public ReadRegistersResponse ReadWriteMultipeRegisters(ushort writeStartAddress, short[] registerdata, ushort readStartAddress, ushort readQuantity)
        {
            CheckQuantity(readQuantity, 0x7D); // 125
            CheckQuantity(registerdata.Length, 0x79); // 121
            BuidRequest(
                () => WriteReadWriteMultipleRegistersRequestPDU(writeStartAddress, registerdata, readStartAddress, readQuantity));
            SendRequest();
            var received = ReceiveResponse();
            //TODO: bytecount ignored for the moment - it is the first byte in received
            return new ReadRegistersResponse() { StartingAddress = readStartAddress, Quantity = readQuantity, Payload = received.Slice(1) };
        }
        #endregion
        #endregion

        #region private methods
        #region generic read and write functions
        private void GenericWriteFunction(Action PDUWriteAction)
        {
            BuidRequest(PDUWriteAction);
            SendRequest();
            ReceiveResponse();
        }

        private ReadRegistersResponse GenericReadRegisters(ushort startingAddress, ushort quantity, FunctionCodes functionCode)
        {
            CheckQuantity(quantity, 0x7D); // 125
            BuidRequest(
                () => WriteGenericMultiReadRequestPDU(startingAddress, quantity, functionCode));
            SendRequest();
            var received = ReceiveResponse();
            //TODO: bytecount ignored for the moment - it is the first byte in received
            return new ReadRegistersResponse() { StartingAddress = startingAddress, Quantity = quantity, TransactionId = LastTransactionId, Payload = received.Slice(1) };
        }

        private ReadBitfieldResponse GenericReadBitField(ushort startingAddress, ushort quantity, FunctionCodes functionCode)
        {
            CheckQuantity(quantity, 0x07D0); // 2000 
            BuidRequest(
                () => WriteGenericMultiReadRequestPDU(startingAddress, quantity, functionCode));
            SendRequest();
            var received = ReceiveResponse();
            //TODO: bytecount ignored for the moment - it is the first byte in received
            return new ReadBitfieldResponse() { StartingAddress = startingAddress, Quantity = quantity, Payload = received.Slice(1) };
        }
        #endregion

        #region request creation and handling
        private void BuidRequest(Action PDUWriteAction)
        {
            // First, skip the header and write the PDU
            _writer.Seek(ADUHeaderSize, SeekOrigin.Begin);
            PDUWriteAction();

            // then extract relevant sizes
            _requestPayloadSize = _writer.BaseStream.Position - ADUHeaderSize + 1;
            _requestSize = _writer.BaseStream.Position;

            // Finally write the APDUHeader
            _writer.Seek(0, SeekOrigin.Begin);
            _writer.Write((ushort)GetTransactionId());
            _writer.Write((ushort)0);
            _writer.Write((ushort)_requestPayloadSize);
            _writer.Write((byte)UnitIdentifier);
        }

        private void SendRequest()
        {
            _tcpConnection.Write(_writeBuffer, 0, (int)_requestSize);
            _logger?.Info("Data sent: " + BitConverter.ToString(_writeBuffer, 0, (int)_requestSize));
        }

        private Span<byte> ReceiveResponse()
        {
            int readTotal = 0;
            bool headerParsed = false;

            do
            {
                readTotal += _tcpConnection.Read(_readBuffer, readTotal, _readBuffer.Length - readTotal);
                if ((readTotal >= 6) && !headerParsed)
                {
                    headerParsed = true;
                    ushort payloadLength = ReadAndVerifyMBAPHeader();
                    if (readTotal - 6 >= payloadLength) // At the moment we ignore if more data is received than expected
                    {
                        break;
                    }
                }
            } while (_tcpConnection.DataAvailable);
            _logger?.Info("Data recv: " + BitConverter.ToString(_readBuffer, 0, readTotal));

            ReadAndVerifyFunctionCode();
            return _readBuffer.AsSpan(8, readTotal - 8);

            ushort ReadAndVerifyMBAPHeader()
            {
                _reader.Seek(0);
                var transactionId = _reader.ReadUInt16();   // Skip transaction ID
                var protocolId = _reader.ReadUInt16();
                var payloadLength = _reader.ReadUInt16();
                var unitId = _reader.ReadByte();     // skip Unit ID

                if (protocolId != 0)
                    throw new ModbusProtocolException((byte)ResponseExceptionCodes.IllegalProtocolId);
                if (transactionId != LastTransactionId)
                    throw new ModbusProtocolException((byte)ResponseExceptionCodes.IllegalTransactionId);
                if (unitId != UnitIdentifier)
                    throw new ModbusProtocolException((byte)ResponseExceptionCodes.IllegalUnitId);
                return payloadLength;
            }

            void ReadAndVerifyFunctionCode()
            {
                var functionCode = _reader.ReadByte();
                if ((functionCode & (byte)FunctionCodes.ErrorFlag) == (int)FunctionCodes.ErrorFlag)
                    throw new ModbusProtocolException(_reader.ReadByte());
            }
        }
        #endregion

        #region small helpers
        private ushort GetTransactionId()
        {
            lock (_transactionIdLock)
            {
                return _transactionId++;
            }
        }

        private static void CheckQuantity(int currentQuantity, ushort maxQuantity)
        {
            if (currentQuantity <= 0 || currentQuantity > maxQuantity)
                throw new ArgumentOutOfRangeException(String.Format(Resources.ErrorMessages.Ex_ArgumentOutOfRange_QuantityHigherThan, maxQuantity));
        }
        #endregion
        #endregion
    }
}