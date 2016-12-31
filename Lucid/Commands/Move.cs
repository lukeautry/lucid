using System.Threading.Tasks;
using Lucid.Broadcasts;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;
using Room = Lucid.Models.Room;

namespace Lucid.Commands
{
	public abstract class Move : Command
	{
		private readonly ISessionUserService _sessionUserService;
		private readonly IUserRepository _userRepository;
		private readonly IRoomBroadcaster _roomBroadcaster;
		private readonly IUserMessageQueue _userMessageQueue;

		protected Move(
			string[] keys,
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(keys, redisProvider)
		{
			_sessionUserService = sessionUserService;
			_userRepository = userRepository;
			_roomBroadcaster = roomBroadcaster;
			_userMessageQueue = userMessageQueue;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var user = await _sessionUserService.GetCurrentUser(sessionId);
			var currentRoom = await _sessionUserService.GetCurrentRoom(sessionId);

			var destinationRoomId = GetDestinationRoomId(currentRoom);
			if (!destinationRoomId.HasValue)
			{
				await _userMessageQueue.Enqueue(sessionId, b => b.Add("You cannot go that way."));
				return;
			}

			// TODO: Do things like check if there is some obstruction, if the room is full, etc
			await _userRepository.Modify(user.Id, u => u.CurrentRoomId = destinationRoomId.Value);

			await _roomBroadcaster.Broadcast(destinationRoomId.Value, $"{user.Name} enters from the {ReverseDirection.ToLower()}.", u => u.User.Id != user.Id);
			await _roomBroadcaster.Broadcast(currentRoom.Id, $"{user.Name} leaves to the {Direction.ToLower()}.", u => u.User.Id != user.Id);

			await Look.ShowCurrentRoom(RedisProvider, _sessionUserService, sessionId);
		}

		protected abstract int? GetDestinationRoomId(Room currentRoom);
		protected abstract string Direction { get; }
		protected abstract string ReverseDirection { get; }
	}

	public sealed class East : Move
	{
		public East(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(new[] { "e", "ea", "eas", "east" }, redisProvider, sessionUserService, userRepository, roomBroadcaster, userMessageQueue) { }

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
		public South(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(new[] { "s", "so", "sou", "sout", "south" }, redisProvider, sessionUserService, userRepository, roomBroadcaster, userMessageQueue) { }

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
		public West(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(new[] { "w", "we", "wes", "west" }, redisProvider, sessionUserService, userRepository, roomBroadcaster, userMessageQueue) { }

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
		public North(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(new[] { "n", "no", "nor", "nort", "north" }, redisProvider, sessionUserService, userRepository, roomBroadcaster, userMessageQueue) { }

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
		public Up(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(new[] { "u", "up" }, redisProvider, sessionUserService, userRepository, roomBroadcaster, userMessageQueue) { }

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
		public Down(
			IRedisProvider redisProvider,
			ISessionUserService sessionUserService,
			IUserRepository userRepository,
			IRoomBroadcaster roomBroadcaster,
			IUserMessageQueue userMessageQueue
			) : base(new[] { "d", "do", "dow", "down" }, redisProvider, sessionUserService, userRepository, roomBroadcaster, userMessageQueue) { }

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata(Direction, "Attempt to move down.", Keys, new CommandArgument[] { });
		}

		protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.DownRoomId; }
		protected override string Direction => "Down";
		protected override string ReverseDirection => "Up";
	}
}
