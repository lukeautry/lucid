using Lucid.Core;

namespace Lucid.Views
{
    public class Exits : View
    {
	    private readonly Models.Room _room;

	    public Exits(IRedisProvider redisProvider, Models.Room room) : base(redisProvider)
	    {
			_room = room;
	    }

	    public override UserMessageBuilder Compile(UserMessageBuilder builder)
	    {
			var exits = "Exits: ";
			if (_room.NorthRoomId.HasValue) { exits += "north "; }
			if (_room.EastRoomId.HasValue) { exits += "east "; }
			if (_room.SouthRoomId.HasValue) { exits += "south "; }
			if (_room.WestRoomId.HasValue) { exits += "west "; }
			if (_room.UpRoomId.HasValue) { exits += "up "; }
			if (_room.DownRoomId.HasValue) { exits += "down "; }

		    builder.Add(exits);

			return builder;
	    }
    }
}
