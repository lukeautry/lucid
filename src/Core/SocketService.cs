using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lucid.Core
{
    public interface ISocketService
    {
        Task Send(string value);
        void Disconnect();
    }

    public sealed class SocketService : ISocketService
    {
        private readonly TcpClient _tcpClient;

        internal SocketService(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public async Task Send(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            await _tcpClient.GetStream().WriteAsync(data, 0, data.Length);
        }

        public void Disconnect()
        {
            _tcpClient.Dispose();
        }
    }
}