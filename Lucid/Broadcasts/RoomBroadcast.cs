using System;
using System.Linq;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;
using Lucid.Services;

namespace Lucid.Broadcasts
{
	public sealed class RoomBroadcast
	{
		private readonly IRedisProvider _redisProvider;
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		public RoomBroadcast(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository)
		{
			_redisProvider = redisProvider;
			_userRepository = userRepository;
			_roomRepository = roomRepository;
		}

		public async Task Broadcast(int roomId, string message, Func<SessionUser, bool> sessionUserFilterFn = null)
		{
			var sessionUserService = new SessionUserService(_userRepository, _roomRepository, _redisProvider);
			var userMessageQueue = new UserMessageQueue(_redisProvider);

			var users = await sessionUserService.GetRoomUsers(roomId);
			if (sessionUserFilterFn != null) { users = users.Where(sessionUserFilterFn); }

			foreach (var user in users)
			{
				await userMessageQueue.Enqueue(user.Session.Id, b => b.Break().Add(message));
			}
		}
	}
}