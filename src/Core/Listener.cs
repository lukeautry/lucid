using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace Lucid.Core
{
    public class Listener
    {
        private readonly TcpListener _listener;

        public Listener(int port)
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            _listener.Start();
        }

        public async Task Listen()
        {
            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                if (tcpClient == null) { continue; }

                Client.Start(tcpClient);
            }
        }
    }
}
