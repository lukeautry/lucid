using Lucid.Core;

namespace Lucid.Views
{
	public class ScoreSheet : View
	{
		private readonly Models.User _user;

		public ScoreSheet(IRedisProvider redisProvider, Models.User user) : base(redisProvider)
		{
			_user = user;
		}

		public override UserMessageBuilder Compile(UserMessageBuilder builder)
		{
			return builder
				.Break()
				.Add($"Name: {_user.Name}");
		}
	}
}
