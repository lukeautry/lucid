using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Events
{
	public class CommandUnrecognizedEventData
	{
		public readonly string SessionId;

		public CommandUnrecognizedEventData(string sessionId)
		{
			SessionId = sessionId;
		}
	}

	public class CommandUnrecognizedEvent : Event<CommandUnrecognizedEventData>
	{
		private readonly IUserMessageQueue _userMessageQueue;

		public CommandUnrecognizedEvent(IRedisProvider redisProvider, IUserMessageQueue userMessageQueue) : base("command-unrecognized", redisProvider)
		{
			_userMessageQueue = userMessageQueue;
		}

		public override async Task Execute(CommandUnrecognizedEventData data)
		{
			await _userMessageQueue.Enqueue(data.SessionId, b => b.Break().Add("Sorry, that command isn't recognized."));
		}
	}
}
