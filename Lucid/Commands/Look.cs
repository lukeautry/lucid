using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Views;

namespace Lucid.Commands
{
	public class Look : Command
	{
		private readonly UserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		public Look(IRedisProvider redisProvider, IDbConnection connection) : base(new[] { "l", "lo", "loo", "look" }, redisProvider)
		{
			_userRepository = new UserRepository(redisProvider, connection);
			_roomRepository = new RoomRepository(redisProvider, connection);
		}

		public override async Task Process(string sessionId)
		{
			var session = await new SessionService(RedisProvider).Get(sessionId);

			var user = await _userRepository.Get(session.UserId);
			if (!user.CurrentRoomId.HasValue)
			{
				Console.WriteLine($"User {user.Id} doesn't have a current room...that is a problem.");
				return;
			}

			var room = await _roomRepository.Get(user.CurrentRoomId.Value);
			await new Room(RedisProvider, room).Render(sessionId);
		}
	}
}
