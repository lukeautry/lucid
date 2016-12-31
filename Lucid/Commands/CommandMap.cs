using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Lucid.Commands
{
	public static class CommandMap
	{
		private static IReadOnlyDictionary<string, Command> _commandMap;
		private static readonly IList<CommandMetadata> _commandMetadataCollection = new List<CommandMetadata>();

		public static void Initialize(IServiceProvider serviceProvider)
		{
			_commandMap = BuildCommandMap(serviceProvider);
		}

		private static IReadOnlyDictionary<string, Command> BuildCommandMap(IServiceProvider serviceProvider)
		{
			var dictionary = new Dictionary<string, Command>();

			var library = DependencyContext.Default.RuntimeLibraries.First(l => l.Name == "Lucid");
			Assembly.Load(new AssemblyName(library.Name)).GetTypes()
				.Where(t =>
				{
					var typeInfo = t.GetTypeInfo();
					if (typeInfo.IsAbstract) { return false; }

					var baseType = t.GetTypeInfo().BaseType;
					while (baseType != null && baseType != typeof(object))
					{
						if (baseType == typeof(Command)) { return true; }
						baseType = baseType.GetTypeInfo().BaseType;
					}

					return false;
				})
				.ToList()
				.ForEach(t => RegisterCommand(t, dictionary, serviceProvider));

			return dictionary;
		}

		public static Command Find(string command)
		{
			var map = _commandMap;
			Command cmd;
			map.TryGetValue(command, out cmd);

			return cmd;
		}

		private static void RegisterCommand(Type type, IDictionary<string, Command> dictionary, IServiceProvider serviceProvider)
		{
			var command = ActivatorUtilities.CreateInstance(serviceProvider, type) as Command;
			if (command == null) { return; }

			_commandMetadataCollection.Add(command.GetCommandMetadata());

			foreach (var key in command.Keys)
			{
				dictionary.Add(key, command);
			}
		}

		public static IEnumerable<CommandMetadata> GetAll()
		{
			return _commandMetadataCollection;
		}
	}
}
