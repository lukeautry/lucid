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
		private readonly IUserMessageQueue _userMessageQueue;
		private readonly ISessionService _sessionService;
		public const string ConfirmPasswordText = "Please confirm your password:";

		public NewUserPasswordInputEvent(
			IRedisProvider redisProvider,
			IUserMessageQueue userMessageQueue,
			ISessionService sessionService
			) : base("new-user-password-input", redisProvider)
		{
			_userMessageQueue = userMessageQueue;
			_sessionService = sessionService;
		}

		public override async Task Execute(NewUserPasswordInputEventData data)
		{
			var validationResponse = PasswordValidation.ValidatePassword(data.Password);
			if (!validationResponse.IsValid)
			{
				await _userMessageQueue.Enqueue(data.SessionId, b => b.Add(validationResponse.Message));
				return;
			}
			
			await _sessionService.Update(data.SessionId, s =>
			{
				s.CreationData.PasswordInputPending = false;
				s.CreationData.ConfirmPasswordInputPending = true;
				s.CreationData.Password = data.Password;
			});

			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add(ConfirmPasswordText));
		}
	}
}
