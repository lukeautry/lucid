using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Events
{
	public class SerializedEvent<T>
	{
		public string Key { get; set; }
		public T Value { get; set; }
	}

	public abstract class Event<T>
	{
		protected readonly IRedisProvider RedisProvider;

		protected Event(IRedisProvider redisProvider)
		{
			RedisProvider = redisProvider ?? new RedisProvider();
		}

		public abstract string Key { get; set; }
		public abstract Task Execute(T data);

		public async Task Enqueue(T value)
		{
			var serializedEvent = new SerializedEvent<T>
			{
				Key = Key,
				Value = value
			};

			await RedisProvider.Publish(EventQueue.QueueKey, serializedEvent);
		}
	}
}
