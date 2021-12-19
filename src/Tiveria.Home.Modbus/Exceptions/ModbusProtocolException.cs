namespace Tiveria.Home.Modbus
{
    public class ModbusProtocolException : ModbusException
    {
        public ResponseExceptionCodes ResponseExceptionCode { get; init; }
        public ModbusProtocolException(byte code) : base(GetMessageForCode(code)) 
        {
        }

        private static string GetMessageForCode(byte code) => code switch
        {
            (byte)ResponseExceptionCodes.IllegalFunction => Resources.ErrorMessages.Protocol_IllegalFunction,
            (byte)ResponseExceptionCodes.IllegalDataAddress => Resources.ErrorMessages.Protocol_IllegalDataAddress,
            (byte)ResponseExceptionCodes.IllegalDataValue => Resources.ErrorMessages.Protocol_IllegalDataValue,
            (byte)ResponseExceptionCodes.ServerFailure => Resources.ErrorMessages.Protocol_ServerFailure,
            (byte)ResponseExceptionCodes.Acknowledge => Resources.ErrorMessages.Protocol_Acknowledge,
            (byte)ResponseExceptionCodes.ServerBusy => Resources.ErrorMessages.Protocol_ServerBusy,
            (byte)ResponseExceptionCodes.GatewayPathUnavailable => Resources.ErrorMessages.Protocol_GatewayPathUnavailable,
            (byte)ResponseExceptionCodes.GatewayTargetDeviceFailedToRespond => Resources.ErrorMessages.Protocol_GatewayTargetDeviceFailedToRespond,
            (byte)ResponseExceptionCodes.IllegalProtocolId => Resources.ErrorMessages.Protocol_IllegalProtocolId,
            (byte)ResponseExceptionCodes.IllegalTransactionId => Resources.ErrorMessages.Protocol_IllegalTransactionId,
            (byte)ResponseExceptionCodes.IllegalUnitId => Resources.ErrorMessages.Protocol_IllegalUnitId,
            _ => $"Unknow Response Exception Code ({code})!!"
        };
    }
}