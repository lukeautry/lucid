using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lucid.Core
{
	public class SessionData
	{
		public string Id { get; set; }
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

	    public async Task<SessionData> Save(SessionData data)
	    {
			var redis = _redisProvider.GetDatabase();

		    try
		    {
				await redis.StringSetAsync(GetSessionKey(data.Id), JsonConvert.SerializeObject(data));
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return data;
	    }

	    private static string GetSessionKey(string sessionId)
	    {
		    return $"sessions:{sessionId}";
	    }
    }
}