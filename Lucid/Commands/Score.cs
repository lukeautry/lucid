using System;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Views;

namespace Lucid.Commands
{
	public sealed class Score : Command
	{
		private readonly IUserRepository _userRepository;
		private readonly ISessionService _sessionService;

		public Score(
			IRedisProvider redisProvider, 
			IUserRepository userRepository,
			ISessionService sessionService
			) : base(new[] { "sc", "sco", "scor", "score" }, redisProvider)
		{
			_userRepository = userRepository;
			_sessionService = sessionService;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Score", "Get details about your character.", Keys, new CommandArgument[] { });
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var session = await _sessionService.Get(sessionId);
			if (!session.UserId.HasValue)
			{
				Console.WriteLine($"Session {sessionId} attempted to use the score command, but there was no UserId in SessionData");
				return;
			}

			var user = await _userRepository.Get(session.UserId.Value);
			await new ScoreSheet(RedisProvider, user).Render(sessionId);
		}
	}
}
