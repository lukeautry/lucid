using System;
using System.Collections.Generic;
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
		public int? UserId { get; set; }

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

	public interface ISessionService
	{
		Task<SessionData> Initialize();
		Task OnEviction(string sessionId, Action evictionFn);
		Task<SessionData> Get(string sessionId);
		Task<SessionData> Save(SessionData data);
		Task<SessionData> Update(string sessionId, Action<SessionData> updateFunc);
		Task<Dictionary<string, SessionData>> GetSessions();
		Task Evict(string sessionId);
		Task<SessionData> GetSessionByUserId(int userId);
	}

	public sealed class SessionService : ISessionService
	{
		public const string SessionKey = "sessions";
		private readonly IRedisProvider _redisProvider;

		public SessionService(IRedisProvider redisProvider)
		{
			_redisProvider = redisProvider;
		}

		public async Task<SessionData> Initialize()
		{
			var session = new SessionData(Guid.NewGuid().ToString());
			return await Save(session);
		}

		public async Task OnEviction(string sessionId, Action evictionFn)
		{
			await _redisProvider.SubscribeVoid(GetEvictionKey(sessionId), evictionFn);
		}

		private static string GetEvictionKey(string sessionId)
		{
			return $"session-eviction:{sessionId}";
		}

		public async Task<SessionData> Get(string sessionId)
		{
			return await _redisProvider.HashGet<SessionData>(SessionKey, sessionId);
		}

		public async Task<SessionData> Save(SessionData data)
		{
			await _redisProvider.HashSet(SessionKey, data.Id, data);
			return data;
		}

		public async Task<SessionData> Update(string sessionId, Action<SessionData> updateFunc)
		{
			var session = await Get(sessionId);
			updateFunc(session);

			return await Save(session);
		}

		public async Task<Dictionary<string, SessionData>> GetSessions()
		{
			return await _redisProvider.HashGetDictionary<SessionData>(SessionKey);
		}

		public async Task Evict(string sessionId)
		{
			await _redisProvider.HashDelete(SessionKey, sessionId);
			await _redisProvider.PublishVoid(GetEvictionKey(sessionId));
		}

		public async Task<SessionData> GetSessionByUserId(int userId)
		{
			var sessions = await GetSessions();
			foreach (var session in sessions)
			{
				var sessionUserId = session.Value.UserId;
				if (sessionUserId.HasValue && sessionUserId.Value.Equals(userId))
				{
					return session.Value;
				}
			}

			return null;
		}
	}
}