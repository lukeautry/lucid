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

	    public void SetSession(Action<SessionData> updateFunc)
	    {
		    updateFunc(_session);
		    UpdateSession();
		    Console.WriteLine(_session.CreationData.PasswordInputPending);
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
    }
}
