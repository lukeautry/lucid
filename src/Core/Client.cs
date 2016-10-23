using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lucid.Core
{
    public class Client
    {
        private readonly TcpClient _tcpClient;

        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public async Task Start()
        {
            var pendingBuffer = ImmutableArray.Create<byte>();
            while (true)
            {
                var buffer = new byte[1024];
                await _tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);

                var trimmedBuffer = buffer.Where(b => !b.Equals(0)).ToImmutableArray();
                pendingBuffer = pendingBuffer.Concat(trimmedBuffer).ToImmutableArray();

                var indexOfReturn = pendingBuffer.IndexOf(13);
                if (indexOfReturn >= 0)
                {
                    var commandChars = pendingBuffer
                        .Take(indexOfReturn)
                        .Select(b => Convert.ToChar(b))
                        .ToArray();

                    var commandValue = new string(commandChars);
                    await WriteString($"Hey, you just sent the command: {commandValue}");

                    pendingBuffer = ImmutableArray.Create<byte>();
                }
            }
        }

        private async Task WriteString(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            await _tcpClient.GetStream().WriteAsync(data, 0, data.Length);
        }
    }
}
