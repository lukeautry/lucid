using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Core;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Lucid.Tests
{
	public class TestRedisRepository : IRedisProvider
	{
		private readonly Dictionary<string, Queue<string>> _queueDictionary = new Dictionary<string, Queue<string>>();

		public IDatabase GetDatabase()
		{
			var database = new Mock<IDatabase>();
			return database.Object;
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

		public UserMessageData DequeueUserMessage(string sessionId)
		{
			var serializedData = _queueDictionary[UserMessageQueue.GetKey(sessionId)].Dequeue();
			return JsonConvert.DeserializeObject<UserMessageData>(serializedData);
		}
	}
}