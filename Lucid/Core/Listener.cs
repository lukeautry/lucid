using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lucid.Core
{
    public class Listener
    {
        private readonly TcpListener _listener;
	    private readonly IServiceProvider _serviceProvider;

	    public Listener(int port, IServiceProvider serviceProvider)
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            _listener.Start();
	        _serviceProvider = serviceProvider;
        }

        public async Task Listen()
        {
            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                if (tcpClient == null) { continue; }

                new Client(tcpClient, _serviceProvider).Start();
            }
        }
    }
}
