using System;
using System.Threading.Tasks;

namespace Lucid.Core
{
	public class SessionData
	{
		public readonly string Id;
		public bool NameInputPending { get; set; }
		public CreationData CreationData { get; set; }
		public LoginData LoginData { get; set; }

		public SessionData(string id)
		{
			Id = id;
		}
	}

	public class LoginData
	{
		public int UserId { get; set; }
		public string Password { get; set; }
		public bool PasswordInputPending { get; set; }
	}

	public class CreationData
	{
		public string Name { get; set; }
		public bool PasswordInputPending { get; set; }
		public bool ConfirmPasswordInputPending { get; set; }
		public string Password { get; set; }
	}

	public class SessionService
	{
		private readonly IRedisProvider _redisProvider;
		private SessionData _session;

		public SessionService(IRedisProvider redisProvider = null)
		{
			_redisProvider = redisProvider ?? new RedisProvider();
		}

		public async Task<SessionData> Initialize()
		{
			_session = new SessionData(Guid.NewGuid().ToString());
			return await Save(_session);
		}

		public async Task<SessionData> Get(string sessionId)
		{
			if (_session != null) { return _session; }

			_session = await _redisProvider.GetObject<SessionData>(GetSessionKey(sessionId));
			return _session;
		}

		public async Task<SessionData> Save(SessionData data)
		{
			await _redisProvider.SetObject(GetSessionKey(data.Id), data);
			_session = data;

			return data;
		}

		public static string GetSessionKey(string sessionId)
		{
			return $"sessions:{sessionId}";
		}

		public async Task<SessionData> Update(string sessionId, Action<SessionData> updateFunc)
		{
			var session = await Get(sessionId);
			updateFunc(session);

			return await Save(session);
		}
	}
}