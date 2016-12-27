using Lucid.Core;

namespace Lucid.Views
{
	public class Help : View
	{
		public Help(IRedisProvider redisProvider) : base(redisProvider) { }

		public override UserMessageBuilder Compile(UserMessageBuilder builder)
		{
			return builder
				.Break()
				.Add("Usage: help [topic or command], e.g. help look");
		}
	}
}
