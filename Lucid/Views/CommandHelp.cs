using System.Linq;
using Lucid.Core;
using Lucid.Commands;

namespace Lucid.Views
{
	public class CommandHelp : View
	{
		private readonly CommandMetadata _metadata;

		public CommandHelp(IRedisProvider redisProvider, CommandMetadata metadata) : base(redisProvider)
		{
			_metadata = metadata;
		}

		public override UserMessageBuilder Compile(UserMessageBuilder builder)
		{
			return builder
				.Break()
				.Add($"Command: {_metadata.Name}")
				.Add($"Usage: {_metadata.Aliases.Last()} {string.Join(" ", _metadata.Arguments.Select(a => $"[{a.Name}]"))}")
				.Break()
				.Add(_metadata.Description);
		}
	}
}
