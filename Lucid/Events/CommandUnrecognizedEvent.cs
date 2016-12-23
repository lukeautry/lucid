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
		public CommandUnrecognizedEvent(IRedisProvider redisProvider) : base("command-unrecognized", redisProvider) { }

		public override async Task Execute(CommandUnrecognizedEventData data)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);
			await userMessageQueue.Enqueue(data.SessionId, b => b.Add("Sorry, that command isn't recognized.").Break());
		}
	}
}
