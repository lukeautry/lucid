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
		public bool CommandPending { get; set; }

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

		public SessionService(IRedisProvider redisProvider = null)
		{
			_redisProvider = redisProvider ?? new RedisProvider();
		}

		public async Task<SessionData> Initialize()
		{
			var session = new SessionData(Guid.NewGuid().ToString());
			return await Save(session);
		}

		public async Task<SessionData> Get(string sessionId)
		{
			return  await _redisProvider.GetObject<SessionData>(GetSessionKey(sessionId));
		}

		public async Task<SessionData> Save(SessionData data)
		{
			await _redisProvider.SetObject(GetSessionKey(data.Id), data);
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