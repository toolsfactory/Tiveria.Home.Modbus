namespace Tiveria.Home.Modbus
{
    public abstract class ModbusException : Exception
    {
        public ModbusException() { }
        public ModbusException(string message) : base(message) { }
        public ModbusException(string message, Exception inner) : base(message, inner) { }
        protected ModbusException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class ModbusProtocolMissmatchException
    {
        public ModbusProtocolMissmatchException() { }
    }
}