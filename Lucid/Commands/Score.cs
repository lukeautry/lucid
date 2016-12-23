using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;
using Lucid.Views;

namespace Lucid.Commands
{
	public class Score : Command
	{
		private readonly IUserRepository _userRepository;

		public Score(IRedisProvider redisProvider, IUserRepository userRepository) : base(new[] { "sc", "sco", "scor", "score" }, redisProvider)
		{
			_userRepository = userRepository;
		}

		public override async Task Process(string sessionId)
		{
            var session = await new SessionService(RedisProvider).Get(sessionId);
            if (!session.UserId.HasValue) {
                Console.WriteLine($"Session {sessionId} attempted to use the score command, but there was no UserId in SessionData");
                return;
            }

            var user = await _userRepository.Get(session.UserId.Value);
            await new ScoreSheet(RedisProvider, user).Render(sessionId);
		}
	}
}
