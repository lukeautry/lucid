using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Core;
using Newtonsoft.Json;

namespace Lucid.Events
{
	public class SerializedEvent<T>
	{
		public string Key { get; set; }
		public T Value { get; set; }
	}

	public abstract class Event<T>
	{
		public readonly string Key;
		protected readonly IRedisProvider RedisProvider;

		protected Event(string key, IRedisProvider redisProvider)
		{
			Key = key;
			RedisProvider = redisProvider;
		}

		public abstract Task Execute(T data);

		public virtual async Task Enqueue(T value)
		{
			var serializedEvent = new SerializedEvent<T>
			{
				Key = Key,
				Value = value
			};

			await RedisProvider.Publish(EventQueue.QueueKey, serializedEvent);
		}

		public void Register(Dictionary<string, Func<string, Task>> eventMap)
		{
			if (eventMap.ContainsKey(Key))
			{
				throw new Exception($"An event with key '{Key}' has already been registered.");
			}

			eventMap.Add(Key, async data =>
			{
				var serializedEvent = JsonConvert.DeserializeObject<SerializedEvent<T>>(data);
				await Execute(serializedEvent.Value);
			});

			Console.WriteLine($"Event '{Key}' registered.");
		}
	}
}
