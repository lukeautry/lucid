using System;
using Lucid.Core;

namespace Lucid.Views
{
	public class PlayerList : View
	{
		private readonly Models.User[] _users;

		public PlayerList(IRedisProvider redisProvider, Models.User[] users) : base(redisProvider)
		{
			_users = users;
		}

		protected override Func<UserMessageBuilder, UserMessageBuilder> Compile()
		{
			return builder =>
			{
				builder
					.Break()
					.Add("Players online:")
					.Add("------------------------------");

				foreach (var user in _users)
				{
					builder.Add(user.Name);
				}

				return builder;
			};
		}
	}
}
