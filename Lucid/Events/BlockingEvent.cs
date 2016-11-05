using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Events
{
	public class BlockingEventData
	{
		public readonly string SessionId;

		public BlockingEventData(string sessionId)
		{
			SessionId = sessionId;
		}
	}

	/// <summary>
	///	An event where a command is required until processing the next command
	/// </summary>
	public abstract class BlockingEvent<T> : Event<T> where T : BlockingEventData
	{
		protected BlockingEvent(string key, IRedisProvider redisProvider) : base(key, redisProvider) { }

		public override async Task Enqueue(T data)
		{
			await base.Enqueue(data);
			await new SessionService(RedisProvider).Update(data.SessionId, s => s.CommandPending = true);
		}

		public override async Task Execute(T data)
		{
			await ExecuteBlockingEvent(data);
			await new SessionService(RedisProvider).Update(data.SessionId, s => s.CommandPending = false);
		}

		protected abstract Task ExecuteBlockingEvent(T data);
	}
}
