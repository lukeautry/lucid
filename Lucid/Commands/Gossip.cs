using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Services;
using System.Linq;

namespace Lucid.Commands
{
	public sealed class Gossip : Command
	{
		private readonly ISessionUserService _sessionUserService;
		private readonly IUserMessageQueue _userMessageQueue;
		private readonly ISessionService _sessionService;

		public Gossip(
			IRedisProvider redisProvider, 
			ISessionUserService sessionUserService, 
			IUserMessageQueue userMessageQueue,
			ISessionService sessionService
			) : base(new[] { "go", "gos", "goss", "gossi", "gossip" }, redisProvider)
		{
			_sessionUserService = sessionUserService;
			_userMessageQueue = userMessageQueue;
			_sessionService = sessionService;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Gossip", "Say something to everyone in the realm (out of character)", Keys, new[] {
				new CommandArgument("message", true)
			});
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var message = string.Join(" ", arguments);

			if (arguments.Length == 0 || string.IsNullOrWhiteSpace(message))
			{
				await _userMessageQueue.Enqueue(sessionId, b => b.Add("Gossip what, exactly?").Break());
				return;
			}

			await _userMessageQueue.Enqueue(sessionId, b => b
				.Break()
				.Add($"[gossip] You: '{message}'"));

			var currentUser = await _sessionUserService.GetCurrentUser(sessionId);
			var allSessions = await _sessionService.GetSessions();

			var sessions = allSessions.Where(s => s.Key != sessionId);
			foreach (var session in sessions)
			{
				await _userMessageQueue.Enqueue(session.Key, b => b
					.Break()
					.Add($"[gossip] {currentUser.Name}: '{message}'"));
			}
		}
	}
}
