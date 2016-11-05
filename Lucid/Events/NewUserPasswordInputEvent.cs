using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Events
{
	public class NewUserPasswordInputEventData : BlockingEventData
	{
		public readonly string Password;

		public NewUserPasswordInputEventData(string sessionId, string password) : base(sessionId)
		{
			Password = password;
		}
	}

	public class NewUserPasswordInputEvent : Event<NewUserPasswordInputEventData>
	{
		public const string ConfirmPasswordText = "Please confirm your password:";

		public NewUserPasswordInputEvent(IRedisProvider redisProvider = null) : base("new-user-password-input", redisProvider) { }

		public override async Task Execute(NewUserPasswordInputEventData data)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);

			var validationResponse = PasswordValidation.ValidatePassword(data.Password);
			if (!validationResponse.IsValid)
			{
				await userMessageQueue.Enqueue(data.SessionId, b => b.Add(validationResponse.Message));
				return;
			}

			var sessionService = new SessionService(RedisProvider);
			await sessionService.Update(data.SessionId, s =>
			{
				s.CreationData.PasswordInputPending = false;
				s.CreationData.ConfirmPasswordInputPending = true;
				s.CreationData.Password = data.Password;
			});

			await userMessageQueue.Enqueue(data.SessionId, b => b.Add(ConfirmPasswordText));
		}
	}
}
