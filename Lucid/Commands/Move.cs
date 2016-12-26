using System.Threading.Tasks;
using Lucid.Broadcasts;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;
using Lucid.Services;

namespace Lucid.Commands
{
	public abstract class Move : Command
	{
		protected readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		protected Move(string[] keys, IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(keys, redisProvider)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var sessionUserService = new SessionUserService(_userRepository, _roomRepository, RedisProvider);

			var user = await sessionUserService.GetCurrentUser(sessionId);
			var currentRoom = await sessionUserService.GetCurrentRoom(sessionId);

			var destinationRoomId = GetDestinationRoomId(currentRoom);
			if (!destinationRoomId.HasValue)
			{
				await new UserMessageQueue(RedisProvider).Enqueue(sessionId, b => b.Add("You cannot go that way."));
				return;
			}

			// TODO: Do things like check if there is some obstruction, if the room is full, etc
			await _userRepository.Modify(user.Id, u => u.CurrentRoomId = destinationRoomId.Value);

			var roomBroadcast = new RoomBroadcast(RedisProvider, _userRepository, _roomRepository);
			await roomBroadcast.Broadcast(destinationRoomId.Value, $"{user.Name} enters from the {ReverseDirection.ToLower()}.", u => u.User.Id != user.Id);
			await roomBroadcast.Broadcast(currentRoom.Id, $"{user.Name} leaves to the {Direction.ToLower()}.", u => u.User.Id != user.Id);

			var destinationRoom = await _roomRepository.Get(destinationRoomId.Value);
			await new Views.Room(RedisProvider, destinationRoom).Render(sessionId);
		}

		protected abstract int? GetDestinationRoomId(Room currentRoom);
		protected abstract string Direction { get; }
		protected abstract string ReverseDirection { get; }
	}

	public sealed class East : Move
	{
		public East(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "e", "ea", "eas", "east" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move east.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.EastRoomId; }
		protected override string Direction => "East";
		protected override string ReverseDirection => "West";
	}

	public sealed class South : Move
	{
		public South(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "s", "so", "sou", "sout", "south" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move south.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.SouthRoomId; }
		protected override string Direction => "South";
		protected override string ReverseDirection => "North";
	}

	public sealed class West : Move
	{
		public West(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "w", "we", "wes", "west" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move west.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.WestRoomId; }
		protected override string Direction => "West";
		protected override string ReverseDirection => "East";
	}

	public sealed class North : Move
	{
		public North(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "n", "no", "nor", "nort", "north" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move north.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.NorthRoomId; }
		protected override string Direction => "North";
		protected override string ReverseDirection => "South";
	}

	public sealed class Up : Move
	{
		public Up(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "u", "up" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move up.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.UpRoomId; }
		protected override string Direction => "Up";
		protected override string ReverseDirection => "Down";
	}

	public sealed class Down : Move
	{
		public Down(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "d", "do", "dow", "down" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move down.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.DownRoomId; }
		protected override string Direction => "Down";
		protected override string ReverseDirection => "Up";
	}
}
