using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Core;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;

namespace Lucid.Tests
{
	public class TestRedisRepository : IRedisProvider
	{
		private readonly Dictionary<string, Queue<string>> _queueDictionary = new Dictionary<string, Queue<string>>();
		private readonly Dictionary<string, string> _staticDictionary = new Dictionary<string, string>();
		private readonly Dictionary<string, Dictionary<string, string>> _hashDictionary = new Dictionary<string, Dictionary<string, string>>();

		public IDatabase GetDatabase()
		{
			var database = new Mock<IDatabase>();
			return database.Object;
		}

		public async Task<string> GetString(string key)
		{
			return _staticDictionary[key];
		}

		public async Task SetString(string key, string value)
		{
			_staticDictionary[key] = value;
		}

		public async Task<T> GetObject<T>(string key)
		{
			var serializedData = await GetString(key);
			return JsonConvert.DeserializeObject<T>(serializedData);
		}

		public async Task SetObject<T>(string key, T value)
		{
			var serializedData = JsonConvert.SerializeObject(value);
			await SetString(key, serializedData);
		}

		public async Task SubscribeString(string key, Action<string> onPublish)
		{

		}

		public async Task Subscribe<T>(string key, Action<T> onPublish)
		{

		}

		public async Task Publish<T>(string key, T data)
		{
			Queue<string> queue;
			var queueExists = _queueDictionary.TryGetValue(key, out queue);
			if (!queueExists)
			{
				queue = new Queue<string>();
				_queueDictionary[key] = queue;
			}

			var content = typeof (T) == typeof (string) ? data as string : JsonConvert.SerializeObject(data);
			queue.Enqueue(content);
		}

		public async Task HashSet<T>(string hashKey, string key, T value)
		{
			Dictionary<string, string> dict;
			var success = _hashDictionary.TryGetValue(hashKey, out dict);
			if (!success)
			{
				_hashDictionary[hashKey] = dict = new Dictionary<string, string>();
			}

			dict[key] = JsonConvert.SerializeObject(value);
		}

		public async Task<T> HashGet<T>(string hashKey, string key)
		{
			var data = _hashDictionary[hashKey][key];
			return JsonConvert.DeserializeObject<T>(data);
		}

		public async Task HashDelete(string hashKey, string key)
		{
			_hashDictionary[hashKey].Remove(key);
		}

		public async Task<IEnumerable<string>> HashGetKeys(string hashKey)
		{
			return _hashDictionary[hashKey].Select(kvp => kvp.Key);
		}

		public async Task<IEnumerable<T>> HashGetValues<T>(string hashKey)
		{
			return _hashDictionary[hashKey].Select(kvp => JsonConvert.DeserializeObject<T>(kvp.Value));
		}

		public async Task<Dictionary<string, T>> HashGetDictionary<T>(string hashKey)
		{
			var keys = await HashGetKeys(hashKey);

			var dictionary = new Dictionary<string, T>();
			foreach (var key in keys)
			{
				var value = await HashGet<T>(hashKey, key);
				dictionary.Add(key, value);
			}

			return dictionary;
		}

		public async Task Reset()
		{

		}

		public UserMessageData DequeueUserMessage(string sessionId)
		{
			var serializedData = _queueDictionary[UserMessageQueue.GetKey(sessionId)].Dequeue();
			return JsonConvert.DeserializeObject<UserMessageData>(serializedData);
		}
	}
}