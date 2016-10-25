using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Lucid.Core
{
	public interface IRedisProvider
	{
		IDatabase GetDatabase();
		Task SubscribeString(string key, Action<string> onPublish);
		Task Subscribe<T>(string key, Action<T> onPublish);
		Task Publish<T>(string key, T command);
	}

	public class RedisProvider : IRedisProvider
	{
		private static ConnectionMultiplexer _redis;
		private static ConnectionMultiplexer Redis => _redis ?? (_redis = ConnectionMultiplexer.Connect("127.0.0.1"));

		public IDatabase GetDatabase()
		{
			return Redis.GetDatabase();
		}

		public async Task SubscribeString(string key, Action<string> onPublish)
		{
			var subscriber = Redis.GetSubscriber();
			await subscriber.SubscribeAsync(key, (channel, message) =>
			{
				onPublish(message);
			});
		}

		public async Task Subscribe<T>(string key, Action<T> onPublish)
		{
			var subscriber = Redis.GetSubscriber();
			await subscriber.SubscribeAsync(key, (channel, message) =>
			{
				var resolvedObject = JsonConvert.DeserializeObject<T>(message);
				onPublish(resolvedObject);
			});
		}

		public async Task Publish<T>(string key, T command)
		{
			var serializedData = typeof(T) == typeof(string)
				? command as string
				: JsonConvert.SerializeObject(command);

			var subscriber = Redis.GetSubscriber();
			await subscriber.PublishAsync(key, serializedData);
		}
	}
}