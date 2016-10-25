using System.Threading.Tasks;

namespace Lucid.Core
{
	public sealed class UserMessageData
	{
		public string Content { get; set; }
	}

    public sealed class UserMessageQueue
    {
	    private readonly IRedisProvider _redisProvider;
		private readonly string _key;

	    public UserMessageQueue(string sessionId, IRedisProvider redisProvider = null)
	    {
			_key = $"user-messages:{sessionId}";
			_redisProvider = redisProvider ?? new RedisProvider();
	    }

	    public async Task Enqueue(UserMessageData data)
	    {
		    await _redisProvider.Publish(_key, data);
	    }

	    public async Task Start(ISocketService socketService)
	    {
		    await _redisProvider.Subscribe<UserMessageData>(_key, data =>
		    {
				socketService.Send(data.Content);
		    });
	    }
    }
}
