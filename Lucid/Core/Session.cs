using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lucid.Core
{
    public class SessionData
    {
        public string Id { get; set; }
	    public bool NameInputPending { get; set; }
    }

    public class Session
    {
        private readonly IRedisProvider _redisProvider;

        public Session(IRedisProvider redisProvider = null)
        {
            _redisProvider = redisProvider ?? new RedisProvider();
        }

        public async Task<SessionData> Initialize()
        {
            var session = new SessionData
            {
                Id = Guid.NewGuid().ToString()
            };

            return await Save(session);
        }

	    public async Task<SessionData> Get(string sessionId)
	    {
		    var session = await _redisProvider.GetDatabase().StringGetAsync(GetSessionKey(sessionId));
		    return JsonConvert.DeserializeObject<SessionData>(session);
	    }

        public async Task<SessionData> Save(SessionData data)
        {
            await _redisProvider.GetDatabase().StringSetAsync(GetSessionKey(data.Id), JsonConvert.SerializeObject(data));
			return data;
        }

        private static string GetSessionKey(string sessionId)
        {
            return $"sessions:{sessionId}";
        }
    }
}