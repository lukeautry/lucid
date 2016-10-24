using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lucid.Core
{
	public static class Client
	{
		public static async Task Start(TcpClient tcpClient)
		{
			var socketService = new SocketService(tcpClient);
			var session = await Session.Initialize();
			var commandQueue = CommandQueue.Initialize(session.Id);

			var pendingBuffer = ImmutableArray.Create<byte>();
			while (true)
			{
				var buffer = new byte[1024];
				await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);

				var trimmedBuffer = buffer.Where(b => !b.Equals(0)).ToImmutableArray();
				pendingBuffer = pendingBuffer.Concat(trimmedBuffer).ToImmutableArray();

				var indexOfReturn = pendingBuffer.IndexOf(13);
				if (indexOfReturn < 0) { continue; }
				
				var commandChars = pendingBuffer
					.Take(indexOfReturn)
					.Select(Convert.ToChar)
					.ToArray();

				var commandValue = new string(commandChars);
				if (!string.IsNullOrEmpty(commandValue))
				{

				}

				pendingBuffer = ImmutableArray.Create<byte>();
			}
		}
	}

	public class CommandQueue
	{
		public static async CommandQueue Initialize(string sessionId)
		{

		}
	}
}
