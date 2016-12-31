using System.Threading.Tasks;
using Lucid.Broadcasts;
using Lucid.Core;
using Lucid.Services;

namespace Lucid.Commands
{
	public sealed class Say : Command
	{
		private readonly ISessionUserService _sessionUserService;
		private readonly IUserMessageQueue _userMessageQueue;
		private readonly IRoomBroadcaster _roomBroadcaster;

		public Say(
			IRedisProvider redisProvider, 
			ISessionUserService sessionUserService,
			IUserMessageQueue userMessageQueue,
			IRoomBroadcaster roomBroadcaster) : base(new[] { "say" }, redisProvider)
		{
			_sessionUserService = sessionUserService;
			_userMessageQueue = userMessageQueue;
			_roomBroadcaster = roomBroadcaster;
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Say", "Say something to the room.", Keys, new[] {
				new CommandArgument("message", true)
			});
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var message = string.Join(" ", arguments);

			if (arguments.Length == 0 || string.IsNullOrWhiteSpace(message))
			{
				await _userMessageQueue.Enqueue(sessionId, b => b.Add("Say what, exactly?").Break());
				return;
			}

			await _userMessageQueue.Enqueue(sessionId, b => b
				.Break()
				.Add($"You say, '{message}'"));
			
			var user = await _sessionUserService.GetCurrentUser(sessionId);
			var room = await _sessionUserService.GetCurrentRoom(sessionId);

			await _roomBroadcaster.Broadcast(room.Id, $"{user.Name} says, '{message}'", u => u.User.Id != user.Id);
		}
	}
}
