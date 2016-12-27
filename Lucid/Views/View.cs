using System;
using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Views
{
	public abstract class View
	{
		protected readonly IRedisProvider RedisProvider;

		protected View(IRedisProvider redisProvider)
		{
			RedisProvider = redisProvider;
		}

		public async Task Render(string sessionId)
		{
			await new UserMessageQueue(RedisProvider).Enqueue(sessionId, Compile);
		}

		public abstract UserMessageBuilder Compile(UserMessageBuilder builder);
	}
}
