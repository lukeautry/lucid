using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lucid.Core
{
	public class SessionData
	{
		public string Id { get; set; }
	}

    public static class Session
    {
        public static async Task<SessionData> Initialize()
        {
	        var session = new SessionData
	        {
		        Id = Guid.NewGuid().ToString()
	        };

	        return await Save(session);
        }

	    public static async Task<SessionData> Save(SessionData data)
	    {
			var redis = RedisProvider.Get();
		    await redis.StringSetAsync(GetSessionKey(data.Id), JsonConvert.SerializeObject(data));

			return data;
	    }

	    private static string GetSessionKey(string sessionId)
	    {
		    return $"sessions:{sessionId}";
	    }
    }
}