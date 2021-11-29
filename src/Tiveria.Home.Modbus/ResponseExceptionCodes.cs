namespace Tiveria.Home.Modbus
{
    /// <summary>
    /// Modbus exception codes sent by a server in case the request is not correct.
    /// <see cref="https://modbus.org/docs/Modbus_Application_Protocol_V1_1b3.pdf"/> Chapter 7
    /// </summary>
    public enum ResponseExceptionCodes : byte
    {
        IllegalFunction = 0x01,
        IllegalDataAddress = 0x02,
        IllegalDataValue = 0x03,
        ServerFailure = 0x04,
        Acknowledge = 0x05,
        ServerBusy = 0x06,
        MemoryParityError = 0x8,
        GatewayPathUnavailable = 0x0A,
        GatewayTargetDeviceFailedToRespond = 0x0B,

        IllegalUnitId = 0xFE,
        IllegalTransactionId = 0xFD,
        IllegalProtocolId = 0xFF
    }

}