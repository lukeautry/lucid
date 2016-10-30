using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Lucid.Core
{
	public interface IRedisProvider
	{
		Task<string> GetString(string key);
		Task SetString(string key, string value);
		Task<T> GetObject<T>(string key);
		Task SetObject<T>(string key, T value);
		Task SubscribeString(string key, Action<string> onPublish);
		Task Subscribe<T>(string key, Action<T> onPublish);
		Task Publish<T>(string key, T data);
	}

	public class RedisProvider : IRedisProvider
	{
		private static ConnectionMultiplexer _redis;
		private static ConnectionMultiplexer Redis => _redis ?? (_redis = ConnectionMultiplexer.Connect("127.0.0.1"));

		public async Task<string> GetString(string key)
		{
			return await Redis.GetDatabase().StringGetAsync(key);
		}

		public async Task SetString(string key, string value)
		{
			await Redis.GetDatabase().StringSetAsync(key, value);
		}

		public async Task<T> GetObject<T>(string key)
		{
			var rawObject = await GetString(key);
			return rawObject == null ? default(T) : JsonConvert.DeserializeObject<T>(await GetString(key));
		}

		public async Task SetObject<T>(string key, T value)
		{
			await SetString(key, JsonConvert.SerializeObject(value));
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

		public async Task Publish<T>(string key, T data)
		{
			var serializedData = typeof(T) == typeof(string)
				? data as string
				: JsonConvert.SerializeObject(data);

			var subscriber = Redis.GetSubscriber();
			await subscriber.PublishAsync(key, serializedData);
		}
	}
}