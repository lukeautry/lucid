using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Commands
{
    public abstract class Command
    {
        public readonly string[] Keys;
        protected readonly IRedisProvider RedisProvider;

        protected Command(string[] keys, IRedisProvider redisProvider)
        {
            Keys = keys;
            RedisProvider = redisProvider;
        }

        public abstract Task Process(string sessionId, string[] arguments);
        public abstract CommandMetadata GetCommandMetadata();

        public async Task DisplayHelpText(string sessionId)
        {
            var metadata = GetCommandMetadata();
            await new Views.CommandHelp(RedisProvider, metadata).Render(sessionId);
        }
    }

    public sealed class CommandMetadata
    {
        public readonly string Name;
		public readonly string Description;
        public readonly string[] Aliases;
        public readonly CommandArgument[] Arguments;

        public CommandMetadata(string name, string description, string[] aliases, CommandArgument[] arguments)
        {
            Name = name;
			Description = description;
            Aliases = aliases;
            Arguments = arguments;
        }
    }

    public sealed class CommandArgument
    {
		public readonly string Name;
		public readonly bool Required;

        public CommandArgument(string name, bool required)
        {
			Name = name;
			Required = required;
        }
    }
}