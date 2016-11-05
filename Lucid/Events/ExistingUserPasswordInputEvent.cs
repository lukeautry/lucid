using System;
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
		private readonly IUserRepository _userRepository;
		private readonly UserMessageQueue _userMessageQueue;
		private readonly IRoomRepository _roomRepository;

		public ExistingUserPasswordInputEvent(
				IUserRepository userRepository = null,
				IRoomRepository roomRepository = null,
				IRedisProvider redisProvider = null
			) : base("existing-user-password-input", redisProvider)
		{
			_userRepository = userRepository ?? new UserRepository();
			_roomRepository = roomRepository ?? new RoomRepository();
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
				Console.WriteLine("Apparently this users currentroomid is invalid.");
				return;
			}

			await _userMessageQueue.Enqueue(sessionId, b => b
				.Break()
				.Add(room.Name)
				.Break()
				.Add(room.Description));
		}
	}
}
