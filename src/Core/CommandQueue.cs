using System.Threading.Tasks;

namespace Lucid.Core
{
	public sealed class CommandQueue
	{
		private readonly string _key;
		private readonly IRedisProvider _redisProvider;
		private readonly CommandProcessor _commandProcessor;
		private readonly string _sessionId;

		public CommandQueue(string sessionId, IRedisProvider redisProvider = null)
		{
			_sessionId = sessionId;
			_key = $"commands:{_sessionId}";
			_redisProvider = redisProvider ?? new RedisProvider();
			_commandProcessor = new CommandProcessor();
		}

		public async Task Enqueue(string command)
		{
			await _redisProvider.Publish(_key, command);
		}

		public async Task<CommandQueue> Start()
		{
			await _redisProvider.SubscribeString(_key, async data =>
			{
				await _commandProcessor.Process(_sessionId, data);
			});

			return this;
		}
	}
}