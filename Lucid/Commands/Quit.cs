using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Commands
{
	public sealed class Quit : Command
	{
		private readonly IUserMessageQueue _userMessageQueue;
		private readonly ISessionService _sessionService;

		public Quit(
			IRedisProvider redisProvider,
			IUserMessageQueue userMessageQueue,
			ISessionService sessionService
			) : base(new[] { "qui", "quit" }, redisProvider)
		{
			_userMessageQueue = userMessageQueue;
			_sessionService = sessionService;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Quit", "Quit the game.", Keys, new CommandArgument[] { });
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			await _userMessageQueue.Enqueue(sessionId, b => b.Add("Quitting..."));
			await _sessionService.Evict(sessionId);
		}
	}
}
