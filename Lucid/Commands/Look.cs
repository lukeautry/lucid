using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Services;
using Lucid.Views;
using System.Linq;

namespace Lucid.Commands
{
	public sealed class Look : Command
	{
		private readonly ISessionUserService _sessionUserService;

		public Look(IRedisProvider redisProvider, ISessionUserService sessionUserService) 
			: base(new[] { "l", "lo", "loo", "look" }, redisProvider)
		{
			_sessionUserService = sessionUserService;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			await ShowCurrentRoom(RedisProvider, _sessionUserService, sessionId);
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Look", "Take a look around or at a target", Keys, new[]{
				new CommandArgument("target", false)
			});
		}

		public static async Task ShowCurrentRoom(IRedisProvider redisProvider, ISessionUserService sessionUserService, string sessionId)
		{
			var user = await sessionUserService.GetCurrentUser(sessionId);
			var room = await sessionUserService.GetCurrentRoom(sessionId);
			var roomUsers = await sessionUserService.GetRoomUsers(room.Id);

			await new Room(redisProvider, 
				new RoomData(room, roomUsers.Where(u => u.User.Id != user.Id).Select(u => u.User).ToArray())).Render(sessionId);
		}
	}
}
