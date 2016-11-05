using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;

namespace Lucid.Events
{
	public class ExistingUserPasswordInputEventData : BlockingEventData
	{
		public readonly string Password;
		public readonly int UserId;

		public ExistingUserPasswordInputEventData(string sessionId, int userId, string password) : base(sessionId)
		{
			UserId = userId;
			Password = password;
		}
	}

	public class ExistingUserPasswordInputEvent : BlockingEvent<ExistingUserPasswordInputEventData>
	{
		private readonly UserRepository _userRepository;
		private readonly UserMessageQueue _userMessageQueue;
		private readonly IRoomRepository _roomRepository;

		public ExistingUserPasswordInputEvent(IRedisProvider redisProvider, IDbConnection connection) : base("existing-user-password-input", redisProvider)
		{
			_userRepository = new UserRepository(RedisProvider, connection);
			_roomRepository = new RoomRepository(RedisProvider, connection);
			_userMessageQueue = new UserMessageQueue(RedisProvider);
		}

		protected override async Task ExecuteBlockingEvent(ExistingUserPasswordInputEventData data)
		{
			var user = await _userRepository.Get(data.UserId);
			if (!PasswordValidation.VerifyPassword(data.Password, user.HashedPassword))
			{
				await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("That password didn't match. Please try again."));
				return;
			}

			await new SessionService(RedisProvider).Update(data.SessionId, s =>
			{
				s.LoginData = null;
				s.UserId = user.Id;
			});

			await ShowCurrentRoom(data.SessionId, user);
		}

		private async Task ShowCurrentRoom(string sessionId, User user)
		{
			if (!user.CurrentRoomId.HasValue)
			{
				Console.WriteLine("Handle if user has no current room ID");
				return;
			}

			var room = await _roomRepository.Get(user.CurrentRoomId.Value);
			if (room == null)
			{
				Console.WriteLine("Apparently this users CurrentRoomId is invalid.");
				return;
			}

			await new Views.Room(RedisProvider, room).Render(sessionId);
		}
	}
}
