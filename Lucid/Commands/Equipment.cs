using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Services;

namespace Lucid.Commands
{
	public class Equipment : Command
	{
		private readonly ISessionUserService _sessionUserService;

		public Equipment(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService
			) : base(new[] { "eq", "equ", "equi", "equip", "equipm", "equipme", "equipmen", "equipment" }, redisProvider)
		{
			_sessionUserService = sessionUserService;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var user = await _sessionUserService.GetCurrentUser(sessionId); 
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Equipment", "View equipped", Keys, new CommandArgument[] { });
		}
	}
}
