using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lucid.Database;
using Lucid.Events;
using Lucid.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lucid.Core
{
	public sealed class Client
	{
		private readonly TcpClient _tcpClient;
		private readonly IRedisProvider _redisProvider;
		private readonly IUserRepository _userRepository;
		private readonly ISessionUserService _sessionUserService;
		private readonly ISessionService _sessionService;
		private readonly IUserMessageQueue _userMessageQueue;

		public Client(TcpClient tcpClient, IServiceProvider serviceProvider)
		{
			_tcpClient = tcpClient;
			_redisProvider = serviceProvider.GetRequiredService<IRedisProvider>();
			_userRepository = serviceProvider.GetRequiredService<IUserRepository>();
			_sessionUserService = serviceProvider.GetRequiredService<ISessionUserService>();
			_sessionService = serviceProvider.GetRequiredService<ISessionService>();
			_userMessageQueue = serviceProvider.GetRequiredService<IUserMessageQueue>();
		}

		public async Task Start()
		{
			var socketService = new SocketService(_tcpClient);
			var session = await _sessionService.Initialize();
			await new ConnectEvent(_redisProvider, _userMessageQueue, _sessionService).Enqueue(new ConnectEventData(session.Id));
			await _userMessageQueue.Start(session.Id, socketService);
			await _sessionService.OnEviction(session.Id, () => _tcpClient.Dispose());

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
						await new CommandProcessor(_redisProvider, _userRepository, _sessionUserService, _sessionService, _userMessageQueue).Process(session.Id, command);
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
				var session = await _sessionService.Get(sessionId);
				if (!session.CommandPending) { return; }
				
				await Task.Delay(100);
			}
		}
	}
}
