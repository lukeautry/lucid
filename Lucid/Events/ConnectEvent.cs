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
		private readonly IUserMessageQueue _userMessageQueue;
		private readonly ISessionService _sessionService;
		public const string WelcomeMessage = "Welcome to LucidMUD!";
		public const string NameInputMessage = "Please enter your name:";

		public ConnectEvent(IRedisProvider redisProvider, IUserMessageQueue userMessageQueue, ISessionService sessionService) : base("connect", redisProvider)
		{
			_userMessageQueue = userMessageQueue;
			_sessionService = sessionService;
		}

		public override async Task Execute(ConnectEventData data)
		{
			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add(WelcomeMessage).Break());
			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add(NameInputMessage));
			await _sessionService.Update(data.SessionId, s =>
			{
				s.NameInputPending = true;
			});
		}
	}
}
