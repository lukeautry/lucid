using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Commands
{
	public class Quit : Command
	{
		public Quit(IRedisProvider redisProvider) : base(new[] { "qui", "quit" }, redisProvider) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Quit", "Quit the game.", Keys, new CommandArgument[] { });
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			await new UserMessageQueue(RedisProvider).Enqueue(sessionId, b => b.Add("Quitting..."));
			await new SessionService(RedisProvider).Evict(sessionId);
		}
	}
}
