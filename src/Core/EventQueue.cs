using System;
using System.Collections.Generic;
using Lucid.Events;
using Newtonsoft.Json;

namespace Lucid.Core
{
	/// <summary>
	/// Generic event queue for global events
	/// </summary>
	public sealed class EventQueue
	{
		public const string QueueKey = "events";
		private readonly IRedisProvider _redisProvider;
		private readonly Dictionary<string, Action<string>> _eventMap = new Dictionary<string, Action<string>>();

		public EventQueue(IRedisProvider redisProvider = null)
		{
			_redisProvider = redisProvider ?? new RedisProvider();
			RegisterEvents();
		}

		public void Start()
		{
			_redisProvider.SubscribeString(QueueKey, data =>
			{
				var dynamicEventData = JsonConvert.DeserializeObject<SerializedEvent<dynamic>>(data);
				var key = dynamicEventData.Key;

				var eventHandler = _eventMap[key];
				eventHandler(data);
			});
		}

		private void RegisterEvents()
		{
			Register(new ConnectEvent());
		}

		private void Register<T>(Event<T> ev)
		{
			_eventMap.Add(ev.Key, data =>
			{
				var deserializedData = JsonConvert.DeserializeObject<SerializedEvent<T>>(data);
				ev.Execute(deserializedData.Value);
			});
		}
	}
}