using System.Threading.Tasks;
using Lucid.Broadcasts;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;
using System.Linq;

namespace Lucid.Commands
{
	public class Gossip : Command
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		public Gossip(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "go", "gos", "goss", "gossi", "gossip" }, redisProvider)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Gossip", "Say something to everyone in the realm (out of character)", Keys, new[] {
				new CommandArgument("message", true)
			});
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);
			var message = string.Join(" ", arguments);

			if (arguments.Length == 0 || string.IsNullOrWhiteSpace(message))
			{
				await userMessageQueue.Enqueue(sessionId, b => b.Add("Gossip what, exactly?").Break());
				return;
			}

			await userMessageQueue.Enqueue(sessionId, b => b
				.Break()
				.Add($"[gossip] You: '{message}'"));

			var currentUser = await new SessionUserService(_userRepository, _roomRepository, RedisProvider).GetCurrentUser(sessionId);
			var allSessions = await new SessionService(RedisProvider).GetSessions();

			var sessions = allSessions.Where(s => s.Key != sessionId);
			foreach (var session in sessions)
			{
				await userMessageQueue.Enqueue(session.Key, b => b
					.Break()
					.Add($"[gossip] {currentUser.Name}: '{message}'"));
			}
		}
	}
}
