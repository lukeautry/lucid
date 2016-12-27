using Lucid.Core;
using Lucid.Models;

namespace Lucid.Views
{
	public class Room : View
	{
		private readonly Models.Room _room;
		private readonly User[] _users;

		public Room(IRedisProvider redisProvider, RoomData roomData) : base(redisProvider)
		{
			_room = roomData.Room;
			_users = roomData.Users;
		}

		public override UserMessageBuilder Compile(UserMessageBuilder builder)
		{
			builder
				.Break()
				.Add(_room.Name)
				.Break()
				.Add(_room.Description)
				.Break();

			new Exits(RedisProvider, _room)
				.Compile(builder)
				.Break();

			foreach (var user in _users)
			{
				builder.Add($"{user.Name} is standing here.");
			}

			return builder.Break();
		}
	}

	public sealed class RoomData
	{
		public readonly Models.Room Room;
		public readonly User[] Users;

		public RoomData(Models.Room room, User[] users)
		{
			Room = room;
			Users = users;
		}
	}
}
