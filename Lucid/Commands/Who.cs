using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;
using Lucid.Views;

namespace Lucid.Commands
{
	public sealed class Who : Command
	{
		private readonly IUserRepository _userRepository;
		private readonly ISessionService _sessionService;

		public Who(IRedisProvider redisProvider, IUserRepository userRepository, ISessionService sessionService) : base(new[] { "wh", "who" }, redisProvider)
		{
			_userRepository = userRepository;
			_sessionService = sessionService;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Who", "Get a list of other players on the server", Keys, new CommandArgument[] { });
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var sessions = await _sessionService.GetSessions();

			var users = new List<User>();

			foreach (var session in sessions)
			{
				var sessionData = session.Value;
				if (!sessionData.UserId.HasValue) { continue; }

				var user = await _userRepository.Get(sessionData.UserId.Value);
				users.Add(user);
			}

			await new PlayerList(RedisProvider, users.ToArray()).Render(sessionId);
		}
	}
}
