using System;
using System.Collections.Generic;
using System.Globalization;
using Lucid.Events;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

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

				Action<string> eventHandler;
				var hasEvent = _eventMap.TryGetValue(key, out eventHandler);
				if (!hasEvent)
				{
					Console.WriteLine($"No event with key '{key}' found.");
					return;
				}

				eventHandler(data);
			});
		}

		private void RegisterEvents()
		{
			var library = DependencyContext.Default.RuntimeLibraries.First(l => l.Name == "Lucid");
			Assembly.Load(new AssemblyName(library.Name)).GetTypes()
				.Where(t =>
				{
					var baseType = t.GetTypeInfo().BaseType;
					if (baseType == null) { return false; }

					var baseTypeInfo = baseType.GetTypeInfo();
					return baseTypeInfo.IsGenericType && baseTypeInfo.GetGenericTypeDefinition() == typeof(Event<>);
				})
				.ToList()
				.ForEach(RegisterEventType);
		}

		private void RegisterEventType(Type eventType)
		{
			var constructor = eventType.GetConstructors().FirstOrDefault();

			dynamic instance;
			if (constructor != null)
			{
				var parameters = constructor.GetParameters();
				var objects = parameters.Select(parameter => Type.Missing).ToArray();

				instance = constructor.Invoke(objects);
			}
			else
			{
				instance = Activator.CreateInstance(eventType);
			}

			if (instance == null)
			{
				throw new Exception($"Failed to create instance of type {eventType.Name}");
			}

			instance.Register(_eventMap);
		}
	}
}