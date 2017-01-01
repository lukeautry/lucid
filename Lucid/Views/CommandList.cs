using Lucid.Commands;
using Lucid.Core;
using System.Linq;

namespace Lucid.Views
{
    public sealed class CommandList : View
    {
	    private readonly CommandMetadata[] _commandMetadatas;

	    public CommandList(IRedisProvider redisProvider, CommandMetadata[] commandMetadatas) : base(redisProvider)
	    {
			_commandMetadatas = commandMetadatas;
	    }

	    public override UserMessageBuilder Compile(UserMessageBuilder builder)
	    {
		    builder
				.Break()
				.Add("Commands")
				.Add(Constants.VisualSeparator);

		    foreach (var command in _commandMetadatas.OrderBy(c => c.Name))
		    {
			    builder.Add($"{command.Name}: {command.Description}");
		    }

			return builder.Break();
	    }
    }
}
