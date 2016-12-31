using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;

namespace Lucid.Commands
{
	public sealed class Exits : Command
	{
		private readonly ISessionUserService _sessionUserService;

		public Exits(IRedisProvider redisProvider, ISessionUserService sessionUserService) : base(new[] { "exits" }, redisProvider)
		{
			_sessionUserService = sessionUserService;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var currentRoom = await _sessionUserService.GetCurrentRoom(sessionId);
			await new Views.Exits(RedisProvider, currentRoom).Render(sessionId);
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Exits", "Get a list of available exits", Keys, new CommandArgument[] { });
		}
	}
}
