using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Lucid.Commands
{
	public class CommandMap
	{
		private static IReadOnlyDictionary<string, Command> _commandMap;
		private readonly IServiceProvider _serviceProvider;

		public CommandMap(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		private IReadOnlyDictionary<string, Command> BuildCommandMap()
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
				.ForEach(t => RegisterCommand(t, dictionary));

			return dictionary;
		}

		public Command Find(string command)
		{
			var map = _commandMap ?? (_commandMap = BuildCommandMap());
			Command cmd;
			map.TryGetValue(command, out cmd);

			return cmd;
		}

		private void RegisterCommand(Type type, IDictionary<string, Command> dictionary)
		{
			var command = ActivatorUtilities.CreateInstance(_serviceProvider, type) as Command;

			foreach (var key in command.Keys)
			{
				dictionary.Add(key, command);
			}
		}
	}
}
