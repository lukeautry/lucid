using System;
using System.Linq;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Services;

namespace Lucid.Broadcasts
{
	public interface IRoomBroadcaster
	{
		Task Broadcast(int roomId, string message, Func<SessionUser, bool> sessionUserFilterFn = null);
	}

	public sealed class RoomBroadcaster : IRoomBroadcaster
	{
		private readonly IUserMessageQueue _userMessageQueue;
		private readonly ISessionUserService _sessionUserService;

		public RoomBroadcaster(IUserMessageQueue userMessageQueue, ISessionUserService sessionUserService)
		{
			_userMessageQueue = userMessageQueue;
			_sessionUserService = sessionUserService;
		}

		public async Task Broadcast(int roomId, string message, Func<SessionUser, bool> sessionUserFilterFn = null)
		{
			var users = await _sessionUserService.GetRoomUsers(roomId);
			if (sessionUserFilterFn != null) { users = users.Where(sessionUserFilterFn); }

			foreach (var user in users)
			{
				await _userMessageQueue.Enqueue(user.Session.Id, b => b.Break().Add(message));
			}
		}
	}
}