using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Events
{
	public class ConnectEventData
	{
		public string SessionId { get; set; }
	}

	public class ConnectEvent : Event<ConnectEventData>
	{
		public override string Key { get; set; } = "connect";

		public ConnectEvent(IRedisProvider redisProvider = null) : base(redisProvider) { }

		public override async Task Execute(ConnectEventData data)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);

			await userMessageQueue.Enqueue(data.SessionId, b => b.Add("Welcome to LucidMUD!").Break());

			var session = await new Session().Get(data.SessionId);
			session.NameInputPending = true;

			await userMessageQueue.Enqueue(data.SessionId, b => b.Add("Please enter your name:"));
		}
	}
}
