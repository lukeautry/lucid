using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;

namespace Lucid.Commands
{
	public abstract class Move : Command
	{
		protected readonly UserRepository _userRepository;
		private readonly RoomRepository _roomRepository;

		protected Move(string[] keys, IRedisProvider redisProvider, IDbConnection connection) : base(keys, redisProvider)
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
				Console.WriteLine("Current user doesn't have a current room ID...");
				return;
			}

			var currentRoom = await _roomRepository.Get(user.CurrentRoomId.Value);

			var destinationRoomId = GetDestinationRoomId(currentRoom);
			if (!destinationRoomId.HasValue)
			{
				await new UserMessageQueue(RedisProvider).Enqueue(sessionId, b => b.Add("You cannot go that way."));
				return;
			}

			// TODO: Do things like check if there is some obstruction, if the room is full, etc
			await _userRepository.Modify(user.Id, u => u.CurrentRoomId = destinationRoomId.Value);
			var destinationRoom = await _roomRepository.Get(destinationRoomId.Value);

			await new Views.Room(RedisProvider, destinationRoom).Render(sessionId);
		}

		protected abstract int? GetDestinationRoomId(Room currentRoom);
	}

	public sealed class East : Move
	{
		public East(IRedisProvider redisProvider, IDbConnection connection) : base(new[] {"e", "ea", "eas", "east"}, redisProvider, connection) { }
		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.EastRoomId; }
	}

	public sealed class South : Move
	{
		public South(IRedisProvider redisProvider, IDbConnection connection) : base(new[] { "s", "so", "sou", "sout", "south" }, redisProvider, connection) { }
		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.SouthRoomId; }
	}

	public sealed class West : Move
	{
		public West(IRedisProvider redisProvider, IDbConnection connection) : base(new[] { "w", "we", "wes", "west" }, redisProvider, connection) { }
		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.WestRoomId; }
	}

	public sealed class North : Move
	{
		public North(IRedisProvider redisProvider, IDbConnection connection) : base(new[] { "n", "no", "nor", "nort", "north" }, redisProvider, connection) { }
		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.NorthRoomId; }
	}

	public sealed class Up : Move
	{
		public Up(IRedisProvider redisProvider, IDbConnection connection) : base(new[] { "u", "up" }, redisProvider, connection) { }
		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.UpRoomId; }
	}

	public sealed class Down : Move
	{
		public Down(IRedisProvider redisProvider, IDbConnection connection) : base(new[] { "d", "do", "dow", "down" }, redisProvider, connection) { }
		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.DownRoomId; }
	}
}
