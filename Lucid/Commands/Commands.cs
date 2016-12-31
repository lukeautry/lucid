using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Views;
using System.Linq;

namespace Lucid.Commands
{
	public sealed class Commands : Command
	{
		public Commands(IRedisProvider redisProvider) : base(new[] { "co", "com", "comm", "comma", "comman", "command", "commands" }, redisProvider) { }

		public override async Task Process(string sessionId, string[] arguments)
		{
			await new CommandList(RedisProvider, CommandMap.GetAll().ToArray()).Render(sessionId);
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Commands", "Get a list of MUD commands", Keys, new CommandArgument[] { });
		}
	}
}
