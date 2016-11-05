using System;
using System.Collections.Generic;
using Lucid.Events;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Lucid.Core
{
	/// <summary>
	/// Generic event queue for global events
	/// </summary>
	public sealed class EventQueue
	{
		public const string QueueKey = "events";
		private readonly IRedisProvider _redisProvider;
		private readonly Dictionary<string, Func<string, Task>> _eventMap = new Dictionary<string, Func<string, Task>>();
		private readonly IServiceProvider _serviceProvider;

		public EventQueue(IServiceProvider serviceProvider)
		{
			_redisProvider = serviceProvider.GetRequiredService<IRedisProvider>();
			_serviceProvider = serviceProvider;
			RegisterEvents();
		}

		public void Start()
		{
			_redisProvider.SubscribeString(QueueKey, async data =>
			{
				var dynamicEventData = JsonConvert.DeserializeObject<SerializedEvent<dynamic>>(data);
				var key = dynamicEventData.Key;

				Func<string, Task> eventHandler;
				var hasEvent = _eventMap.TryGetValue(key, out eventHandler);
				if (!hasEvent)
				{
					Console.WriteLine($"No event with key '{key}' found.");
					return;
				}

				await eventHandler(data);
			});
		}

		private void RegisterEvents()
		{
			var library = DependencyContext.Default.RuntimeLibraries.First(l => l.Name == "Lucid");
			Assembly.Load(new AssemblyName(library.Name)).GetTypes()
				.Where(t =>
				{
					var typeInfo = t.GetTypeInfo();
					if (typeInfo.IsAbstract) { return false; }

					var baseType = t.GetTypeInfo().BaseType;
					if (baseType == null) { return false; }

					var baseTypeInfo = baseType.GetTypeInfo();
					return baseTypeInfo.IsGenericType && (baseTypeInfo.GetGenericTypeDefinition() == typeof(Event<>) || baseTypeInfo.GetGenericTypeDefinition() == typeof(BlockingEvent<>));
				})
				.ToList()
				.ForEach(RegisterEventType);
		}

		private void RegisterEventType(Type eventType)
		{
			dynamic instance = ActivatorUtilities.CreateInstance(_serviceProvider, eventType);
			instance.Register(_eventMap);
		}
	}
}