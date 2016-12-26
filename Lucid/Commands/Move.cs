using System;
using System.Threading.Tasks;
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

            var destinationRoom = await _roomRepository.Get(destinationRoomId.Value);
            await new Views.Room(RedisProvider, destinationRoom).Render(sessionId);
        }

        protected abstract int? GetDestinationRoomId(Room currentRoom);
    }

    public sealed class East : Move
    {
        public East(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "e", "ea", "eas", "east" }, redisProvider, userRepository, roomRepository) { }

        public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("East", "Attempt to move east.", Keys, new CommandArgument[] { });
        }

        protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.EastRoomId; }
    }

    public sealed class South : Move
    {
        public South(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "s", "so", "sou", "sout", "south" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("South", "Attempt to move south.", Keys, new CommandArgument[] { });
        }

        protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.SouthRoomId; }
    }

    public sealed class West : Move
    {
        public West(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "w", "we", "wes", "west" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("West", "Attempt to move west.", Keys, new CommandArgument[] { });
        }

        protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.WestRoomId; }
    }

    public sealed class North : Move
    {
        public North(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "n", "no", "nor", "nort", "north" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("North", "Attempt to move north.", Keys, new CommandArgument[] { });
        }

        protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.NorthRoomId; }
    }

    public sealed class Up : Move
    {
        public Up(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "u", "up" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("Up", "Attempt to move up.", Keys, new CommandArgument[] { });
        }

        protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.UpRoomId; }
    }

    public sealed class Down : Move
    {
        public Down(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "d", "do", "dow", "down" }, redisProvider, userRepository, roomRepository) { }

		public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("Down", "Attempt to move down.", Keys, new CommandArgument[] { });
        }

        protected override int? GetDestinationRoomId(Room currentRoom) { return currentRoom.DownRoomId; }
    }
}
