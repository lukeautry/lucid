using System;
using Lucid.Core;

namespace Lucid.Views
{
	public class Room : View
	{
		private readonly Models.Room _room;

		public Room(IRedisProvider redisProvider, Models.Room room) : base(redisProvider)
		{
			_room = room;
		}

		protected override Func<UserMessageBuilder, UserMessageBuilder> Compile()
		{
			var exits = "Exits: ";
			if (_room.NorthRoomId.HasValue) { exits += "north "; }
			if (_room.EastRoomId.HasValue) { exits += "east "; }
			if (_room.SouthRoomId.HasValue) { exits += "south "; }
			if (_room.WestRoomId.HasValue) { exits += "west "; }
			if (_room.UpRoomId.HasValue) { exits += "up "; }
			if (_room.DownRoomId.HasValue) { exits += "down "; }

			return builder => builder
				.Break()
				.Add(_room.Name)
				.Break()
				.Add(_room.Description)
				.Break()
				.Add(exits);
		}
	}
}
