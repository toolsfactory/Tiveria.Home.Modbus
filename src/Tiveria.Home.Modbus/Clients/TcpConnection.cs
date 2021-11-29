using System.Net;
using System.Net.Sockets;

namespace Tiveria.Home.Modbus
{
    public class TcpConnection : ITcpConnection
    {
        private TcpClient _tcpClient;
        private int _receiveTimeout = 500;
        private int _sendTimeout = 500;
        private NetworkStream? _networkStream = null;

        public bool Connected => _tcpClient.Connected;
        public int ReceiveTimeout 
        { 
            get => _receiveTimeout; 
            set { _receiveTimeout = value; _tcpClient.ReceiveTimeout = _receiveTimeout; } 
        }
        public int SendTimeout
        { 
            get => _sendTimeout; 
            set { _sendTimeout = value; _tcpClient.SendTimeout = _sendTimeout; }
        }
        public int ConnectTimeout { get; set; } = 1000;

        public bool DataAvailable => _networkStream?.DataAvailable ?? false;

        public TcpConnection()
        {
            _tcpClient = new TcpClient();
        }

        public void Close()
        {
            _tcpClient.Close();
        }

        public void Connect(string hostname, int port = 502)
        {
            CreateTcpClient();
            if (!_tcpClient.ConnectAsync(hostname, port).Wait(ConnectTimeout))
                throw new ModbusTCPException("Connect timed out");
            _networkStream = _tcpClient.GetStream();
        }

        public void Connect(IPEndPoint endpoint)
        {
            CreateTcpClient();
            if (!_tcpClient.ConnectAsync(endpoint).Wait(ConnectTimeout))
                throw new ModbusTCPException("Connect timed out");
            _networkStream = _tcpClient.GetStream();
        }

        private void CreateTcpClient()
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
            _tcpClient = new TcpClient();
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (_networkStream == null)
                throw new InvalidOperationException();
            return _networkStream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _networkStream?.Write(buffer, offset, count);
        }
    }
}