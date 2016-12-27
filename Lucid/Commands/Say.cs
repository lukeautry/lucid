using System.Threading.Tasks;
using Lucid.Broadcasts;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;

namespace Lucid.Commands
{
	public class Say : Command
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		public Say(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "say" }, redisProvider)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Say", "Say something to the room.", Keys, new[] {
				new CommandArgument("message", true)
			});
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);
			var message = string.Join(" ", arguments);

			if (arguments.Length == 0 || string.IsNullOrWhiteSpace(message))
			{
				await userMessageQueue.Enqueue(sessionId, b => b.Add("Say what, exactly?").Break());
				return;
			}

			await userMessageQueue.Enqueue(sessionId, b => b
				.Break()
				.Add($"You say, '{message}'"));

			var sessionUserService = new SessionUserService(_userRepository, _roomRepository, RedisProvider);
			var user = await sessionUserService.GetCurrentUser(sessionId);
			var room = await sessionUserService.GetCurrentRoom(sessionId);

			await new RoomBroadcast(RedisProvider, _userRepository, _roomRepository)
				.Broadcast(room.Id, $"{user.Name} says, '{message}'", u => u.User.Id != user.Id);
		}
	}
}
