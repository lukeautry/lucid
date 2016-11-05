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
				
				while (pendingBuffer.Length > 0)
				{
					var commandSequence = pendingBuffer
						.Skip(0)
						.Take(indexOfReturn)
						.Select(Convert.ToChar)
						.ToArray();

					var command = new string(commandSequence);
					if (!string.IsNullOrEmpty(command))
					{
						await CommandProcessor.Process(session.Id, command);
						await CommandPendingCleared(session.Id);
					}

					// Usually line breaks come in with [13,10] - not sure what the 10 is, but it's there sometimes
					var tenIndex = pendingBuffer.IndexOf(10);
					var enterIndex = pendingBuffer.IndexOf(13);
					var sliceIndex = tenIndex > enterIndex ? tenIndex : enterIndex;

					pendingBuffer = pendingBuffer.Skip(sliceIndex + 1).ToImmutableArray();
					indexOfReturn = pendingBuffer.IndexOf(13);
				}

				pendingBuffer = ImmutableArray.Create<byte>();
			}
		}

		public async Task CommandPendingCleared(string sessionId)
		{
			while (true)
			{
				var session = await new SessionService().Get(sessionId);
				if (!session.CommandPending) { return; }
				
				await Task.Delay(100);
			}
		}
	}
}
