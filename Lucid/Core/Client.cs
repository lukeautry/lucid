using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lucid.Events;

namespace Lucid.Core
{
	public sealed class Client
	{
		private readonly TcpClient _tcpClient;

		public Client(TcpClient tcpClient)
		{
			_tcpClient = tcpClient;
		}

		public async Task Start()
		{
			var socketService = new SocketService(_tcpClient);
			var session = await new SessionService().Initialize();
			var commandQueue = await new CommandQueue(session.Id).Start();
			await new ConnectEvent().Enqueue(new ConnectEventData(session.Id));
			await new UserMessageQueue().Start(session.Id, socketService);

			var pendingBuffer = ImmutableArray.Create<byte>();
			while (true)
			{
				var buffer = new byte[1024];
				await _tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);

				var trimmedBuffer = buffer.Where(b => !b.Equals(0)).ToImmutableArray();
				pendingBuffer = pendingBuffer.Concat(trimmedBuffer).ToImmutableArray();

				var indexOfReturn = pendingBuffer.IndexOf(13);
				if (indexOfReturn < 0) { continue; }
				
				var commandChars = pendingBuffer
					.Take(indexOfReturn)
					.Select(Convert.ToChar)
					.ToArray();

				var command = new string(commandChars);
				if (!string.IsNullOrEmpty(command))
				{
					await commandQueue.Enqueue(command);
				}

				pendingBuffer = ImmutableArray.Create<byte>();
			}
		}
	}
}
