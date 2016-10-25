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

	    public override async Task Execute(ConnectEventData data)
	    {
			await new UserMessageQueue(data.SessionId).Enqueue(new UserMessageData
			{
				Content = "Hey you're connected."
			});
		}
	}
}
