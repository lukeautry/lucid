using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;

namespace Lucid.Events
{
	public class ExistingUserPasswordInputEventData
	{
		public readonly string SessionId;
		public readonly string Password;
		public readonly int UserId;

		public ExistingUserPasswordInputEventData(string sessionId, int userId, string password)
		{
			SessionId = sessionId;
			UserId = userId;
			Password = password;
		}
	}

	public class ExistingUserPasswordInputEvent : Event<ExistingUserPasswordInputEventData>
	{
		private readonly IUserRepository _userRepository;
		private readonly UserMessageQueue _userMessageQueue;

		public ExistingUserPasswordInputEvent(IUserRepository userRepository = null, IRedisProvider redisProvider = null) : base("existing-user-password-input", redisProvider)
		{
			_userRepository = userRepository ?? new UserRepository();
			_userMessageQueue = new UserMessageQueue(RedisProvider);
		}

		public override async Task Execute(ExistingUserPasswordInputEventData data)
		{
			var user = await _userRepository.Get(data.UserId);
			if (!PasswordValidation.VerifyPassword(data.Password, user.HashedPassword))
			{
				await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("That password didn't match. Please try again."));
				return;
			}

			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("Valid password."));
		}
	}
}
