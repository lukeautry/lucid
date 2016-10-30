using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Events
{
	public class ConnectEventData
	{
		public readonly string SessionId;

		public ConnectEventData(string sessionId)
		{
			SessionId = sessionId;
		}
	}

	public class ConnectEvent : Event<ConnectEventData>
	{
		public const string WelcomeMessage = "Welcome to LucidMUD!";
		public const string NameInputMessage = "Please enter your name:";

		public ConnectEvent(IRedisProvider redisProvider = null) : base("connect", redisProvider) { }

		public override async Task Execute(ConnectEventData data)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);

			await userMessageQueue.Enqueue(data.SessionId, b => b.Add(WelcomeMessage).Break());
			await new SessionService(RedisProvider).Update(data.SessionId, s => s.NameInputPending = true);
			await userMessageQueue.Enqueue(data.SessionId, b => b.Add(NameInputMessage));
		}
	}
}
