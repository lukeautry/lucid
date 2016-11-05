using System;
using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Views
{
    public abstract class View
    {
	    private readonly IRedisProvider _redisProvider;

	    protected View(IRedisProvider redisProvider)
	    {
		    _redisProvider = redisProvider;
	    }

		public async Task Render(string sessionId)
		{
			await new UserMessageQueue(_redisProvider).Enqueue(sessionId, Compile());
		}

	    protected abstract Func<UserMessageBuilder, UserMessageBuilder> Compile();
    }
}
