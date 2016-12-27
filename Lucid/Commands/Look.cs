using System;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;
using Lucid.Views;
using System.Linq;

namespace Lucid.Commands
{
	public class Look : Command
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		public Look(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "l", "lo", "loo", "look" }, redisProvider)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			await ShowCurrentRoom(_userRepository, _roomRepository, RedisProvider, sessionId);
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Look", "Take a look around or at a target", Keys, new[]{
				new CommandArgument("target", false)
			});
		}

		public static async Task ShowCurrentRoom(IUserRepository userRepository, IRoomRepository roomRepository, IRedisProvider redisProvider, string sessionId)
		{
			var session = await new SessionService(redisProvider).Get(sessionId);
			if (!session.UserId.HasValue)
			{
				// TODO: Handle
				Console.WriteLine($"Session {sessionId} tried to look with no user ID");
				return;
			}

			var user = await userRepository.Get(session.UserId.Value);
			if (!user.CurrentRoomId.HasValue)
			{
				Console.WriteLine($"User {user.Id} doesn't have a current room...that is a problem.");
				return;
			}

			var room = await roomRepository.Get(user.CurrentRoomId.Value);
			var roomUsers = await new SessionUserService(userRepository, roomRepository, redisProvider).GetRoomUsers(room.Id);
			await new Room(redisProvider, 
				new RoomData(room, roomUsers.Where(u => u.User.Id != user.Id).Select(u => u.User).ToArray())).Render(sessionId);
		}
	}
}
