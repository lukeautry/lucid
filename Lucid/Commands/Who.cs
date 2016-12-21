using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;
using Lucid.Views;

namespace Lucid.Commands
{
	public class Who : Command
	{
		private readonly IUserRepository _userRepository;

		public Who(IRedisProvider redisProvider, IUserRepository userRepository) : base(new[] { "wh", "who" }, redisProvider)
		{
			_userRepository = userRepository;
		}

		public override async Task Process(string sessionId)
		{
			var sessions = await new SessionService(RedisProvider).GetSessions();

			var users = new List<User>();

			foreach (var session in sessions)
			{
				if (session.Key.Equals(sessionId, StringComparison.OrdinalIgnoreCase)) { continue; }

				var sessionData = session.Value;
				if (!sessionData.UserId.HasValue) { continue; }

				var user = await _userRepository.Get(sessionData.UserId.Value);
				users.Add(user);
			}

			await new PlayerList(RedisProvider, users.ToArray()).Render(sessionId);
		}
	}
}
