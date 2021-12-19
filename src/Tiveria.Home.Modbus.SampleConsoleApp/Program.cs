using Tiveria.Common;
using Tiveria.Home.Modbus;


var c2 = new ModbusTCPClient(logger: new Tiveria.Common.Logging.ConsoleLogger("ModbusTCPClient"));
//var c2 = new ModbusTCPClient();
//c2.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("192.168.2.157"), 502));
c2.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 502));
var val = c2.ReadHoldingRegistersAsFloat(0);
Console.WriteLine(val);

var data = BitConverter.GetBytes(val);
Console.WriteLine(BitConverter.ToString(data));
Console.WriteLine();
var d2 = (byte[]) data.Clone();
data[3] = 0;
Console.WriteLine(BitConverter.ToString(data));
Console.WriteLine(BitConverter.ToSingle(data));
Console.WriteLine();
data = (byte[])d2.Clone();
data[2] = 255;
Console.WriteLine(BitConverter.ToString(data));
Console.WriteLine(BitConverter.ToSingle(data));
Console.WriteLine();
data = (byte[])d2.Clone();
data[1] = 0;
Console.WriteLine(BitConverter.ToString(data));
Console.WriteLine(BitConverter.ToSingle(data));
Console.WriteLine();
data = (byte[])d2.Clone();
data[0] = 0;
Console.WriteLine(BitConverter.ToString(data));
Console.WriteLine(BitConverter.ToSingle(data));

c2.Disconnect();
/*
c2.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("192.168.2.157"), 502));


Console.WriteLine($"SystemId:         " + c2.ReadHoldingRegistersAsUInt(2));
Console.WriteLine($"Serialno:         " + c2.ReadHoldingRegistersAsULong(4));
Console.WriteLine($"SystemName:       " + c2.ReadHoldingRegistersAsString(8, 14));
var resp = c2.ReadInputRegisters(24, 2);
Console.WriteLine($"FW Version:       {resp.Payload[0]}.{resp.Payload[1]}");

ShowTime(108,      "Currenttime:      ");
ShowTime(668,      "Exploitation:     ");
var worktime = c2.ReadInputRegisters(624, 2).GetRegistersAsUInt(624);
Console.WriteLine($"Worktime:         {worktime} h");
Console.WriteLine($"Worktime:         {worktime/(365*24)} years, {(worktime % (365 * 24)) / 24} days");

Console.WriteLine($"Fan1:             " + c2.ReadHoldingRegistersAsFloat(100));
Console.WriteLine($"Fan2:             " + c2.ReadHoldingRegistersAsFloat(102));

Console.WriteLine($"Outdoor Temp:     " + c2.ReadHoldingRegistersAsFloat(132));
Console.WriteLine($"Supply Temp:      " + c2.ReadHoldingRegistersAsFloat(134));
Console.WriteLine($"Extract Temp:     " + c2.ReadHoldingRegistersAsFloat(136));
Console.WriteLine($"Exhaust Temp:     " + c2.ReadHoldingRegistersAsFloat(138));

resp = c2.ReadInputRegisters(554, 4);
Console.WriteLine($"Filter remaining: {resp.GetRegisterUnsigned(554)} von {resp.GetRegisterUnsigned(556)}");

Console.WriteLine($"Net - DHCP Enable:" + c2.ReadHoldingRegistersAsInt(26));
resp = c2.ReadInputRegisters(28, 2);
Console.WriteLine($"Net - IP:         {resp.Payload[2]}.{resp.Payload[3]}.{resp.Payload[0]}.{resp.Payload[1]}");
resp = c2.ReadInputRegisters(32, 2);
Console.WriteLine($"Net - Mask:       {resp.Payload[2]}.{resp.Payload[3]}.{resp.Payload[0]}.{resp.Payload[1]}");
resp = c2.ReadInputRegisters(36, 2);
Console.WriteLine($"Net - Gateway:    {resp.Payload[2]}.{resp.Payload[3]}.{resp.Payload[0]}.{resp.Payload[1]}");
resp = c2.ReadInputRegisters(40, 4);
Console.WriteLine($"Net - Gateway:    {resp.Payload[0]:x2}:{resp.Payload[1]:x2}:{resp.Payload[6]:x2}:{resp.Payload[7]:x2}:{resp.Payload[4]:x2}:{resp.Payload[5]:x2}");

Console.WriteLine($"HAL Left:         " + c2.ReadHoldingRegistersAsUInt(84));
Console.WriteLine($"HAL Right:        " + c2.ReadHoldingRegistersAsUInt(86));

Console.WriteLine($"Current BL State: " + c2.ReadHoldingRegistersAsUInt(472));
Console.WriteLine($"Last Active Alarm:" + c2.ReadHoldingRegistersAsUInt(516));

Console.WriteLine($"Bypass T Min:     " + c2.ReadHoldingRegistersAsFloat(444));
Console.WriteLine($"Bypass T Max:     " + c2.ReadHoldingRegistersAsFloat(446));
Console.WriteLine($"Bypass act. state:" + c2.ReadHoldingRegistersAsUInt(198));
Console.WriteLine($"Manual Bypass min:" + c2.ReadHoldingRegistersAsUInt(264));

Console.WriteLine($"Preheater duty cy:" + c2.ReadHoldingRegistersAsUInt(160));
Console.WriteLine($"Fan Ref Extract:  " + c2.ReadHoldingRegistersAsUInt(518));
Console.WriteLine($"Fan Ref Supply:   " + c2.ReadHoldingRegistersAsUInt(520));
Console.WriteLine($"Fireplace Preset: " + c2.ReadHoldingRegistersAsUInt(540));


resp = c2.ReadInputRegisters(472, 1);

c2.WriteMultipeRegisters(168, new short[] { 8 });
resp = c2.ReadInputRegisters(472, 1);

c2.Disconnect();



void ShowTime(ushort register, string name)
{
    var epoch = c2.ReadInputRegisters(register, 2).GetRegistersAsUInt(register);
    Console.WriteLine($"{name}{epoch} = {ToDateTimeFromEpoch(epoch)} ({ToEpochTime(DateTime.Now.ToLocalTime())} = {ToDateTimeFromEpoch(ToEpochTime(DateTime.Now))})");
}


static ulong ToEpochTime(DateTime dateTime)
{
    var date = dateTime.ToUniversalTime();
    var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
    var ts = (ulong) (ticks / TimeSpan.TicksPerSecond);
    return ts;
}

/// <summary>
/// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
/// </summary>
static DateTime ToDateTimeFromEpoch(ulong intDate)
{
    long timeInTicks = (long) intDate * TimeSpan.TicksPerSecond;
    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
}

/// <summary>
/// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
/// </summary>
static DateTimeOffset ToDateTimeOffsetFromEpoch(ulong intDate)
{
    var timeInTicks = (long)intDate * TimeSpan.TicksPerSecond;
    return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(timeInTicks);
}
/*

var c2 = new ModbusTCPClient(logger: new Tiveria.Common.Logging.ConsoleLogger("ModbusTCPClient"));
c2.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 502));

var resp2 = c2.ReadInputRegisters(4, 4);
Write16bitValues(resp2);


Console.WriteLine("--------------");
var resp = c2.ReadHoldingRegisters(8, 15);
Write16bitValues(resp);

c2.WriteSingleRegister(9, 7777);

c2.WriteMultipeRegisters(10, new short[] { 8888, 9999, 5555 });

c2.WriteSingleCoil(1, true);
c2.WriteSingleCoil(2, true);
c2.WriteSingleCoil(3, true);
var coils = c2.ReadCoils(0, 25);
WriteCoils(coils);
var coildata = new bool[8] { true, false, true, false, true, false, true, false };

c2.WriteMultipleCoils(0, coildata);
Thread.Sleep(2000);
c2.WriteMultipleCoils(1, coildata);

//var dinp = c2.ReadDiscreteInputs(1, 5);
for (var i = 0; i < resp.Payload.Length / 2; i++)
{
    Console.Write((char)resp.Payload[(i * 2) + 1]);
    Console.Write((char)resp.Payload[(i * 2)]);
}


var resp3 = c2.ReadWriteMultipeRegisters(0, new short[] { 1255, 2255, 3255 }, 0, 6);
Write16bitValues(resp3);

// 00 XX 00 00 00 0B 01 03 08 9F 1B 13 55 01 8C 00 00

    void WriteCoils(ReadBitfieldResponse resp)
    {
        for (var i = resp.StartingAddress; i < resp.StartingAddress + resp.Quantity; i++)
        {
            Console.WriteLine($"{i:D5} : {resp.GetCoil(i)}");
        }
    }

    void Write16bitValues(ReadRegistersResponse resp)
{
    for(var i = resp.StartingAddress; i < resp.StartingAddress+resp.Quantity; i++)
    {
        Console.WriteLine($"{i:D5} : {resp.GetRegister(i):X4} {resp.GetRegister(i)} ({resp.GetRegisterUnsigned(i)})");
    }
}
*/

void CalcSerial(ReadRegistersResponse resp)
{
    long ll = resp.GetRegisterUnsigned(4);
    long lh = resp.GetRegisterUnsigned(5);
    long hl = resp.GetRegisterUnsigned(6);
    long hh = resp.GetRegisterUnsigned(7);
    long serno = (ll + (lh << 16) + (hl << 32));
    Console.WriteLine(serno);
    Console.WriteLine();
}
