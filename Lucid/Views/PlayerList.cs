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

		public override UserMessageBuilder Compile(UserMessageBuilder builder)
		{
			builder
					.Break()
					.Add("Players online:")
					.Add(Constants.VisualSeparator);

			foreach (var user in _users)
			{
				builder.Add(user.Name);
			}

			return builder;
		}
	}
}
