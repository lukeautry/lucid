using System;
using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Tests.Fixtures
{
    public class Context
    {
	    public const string SessionId = "test-session";
		public readonly TestRedisRepository RedisProvider;
	    private SessionData _session;

	    public Context()
	    {
			_session = new SessionData(SessionId);
			RedisProvider = new TestRedisRepository();
			UpdateSession();
	    }

	    public SessionData SetSession(Action<SessionData> updateFunc)
	    {
		    updateFunc(_session);
		    UpdateSession();

			return _session;
	    }

	    public async Task<SessionData> GetSession()
	    {
			_session = await new SessionService(RedisProvider).Get(_session.Id);
			return _session;
		}

	    private void UpdateSession()
	    {
			Task.Run(() => new SessionService(RedisProvider).Save(_session)).Wait();
		}

	    public string GetNextMessage()
	    {
		    return RedisProvider.DequeueUserMessage(_session.Id).Content;
	    }
    }
}
