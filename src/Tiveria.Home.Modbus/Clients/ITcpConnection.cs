using System.Net;

namespace Tiveria.Home.Modbus
{
    public interface ITcpConnection : IDisposable
    {
        bool Connected { get; }
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }
        int ConnectTimeout { get; set; }
        bool DataAvailable { get; }

        void Connect(IPEndPoint endpoint);
        void Connect(string hostname, int port = 502);
        void Close();

        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
    }
}