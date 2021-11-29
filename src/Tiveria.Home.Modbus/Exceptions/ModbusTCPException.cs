namespace Tiveria.Home.Modbus
{
    public class ModbusTCPException : ModbusException
    {
        public ModbusTCPException() { }
        public ModbusTCPException(string message) : base(message) { }
        public ModbusTCPException(string message, Exception inner) : base(message, inner) { }
        protected ModbusTCPException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}