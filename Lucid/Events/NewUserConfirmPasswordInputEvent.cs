using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;

namespace Lucid.Events
{
	public class NewUserConfirmPasswordInputEventData
	{
		public readonly string SessionId;
		public readonly string Password;

		public NewUserConfirmPasswordInputEventData(string sessionId, string password)
		{
			SessionId = sessionId;
			Password = password;
		}
	}

	public class NewUserConfirmPasswordInputEvent : Event<NewUserConfirmPasswordInputEventData>
	{
		private readonly IUserRepository _userRepository;
		public const string NonMatchingPasswordText = "Passwords must match.";

		public NewUserConfirmPasswordInputEvent(IRedisProvider redisProvider = null, IUserRepository userRepository = null)
			: base("new-user-confirm-password-input-event", redisProvider)
		{
			_userRepository = userRepository ?? new UserRepository();
		}

		public override async Task Execute(NewUserConfirmPasswordInputEventData data)
		{
			var userMessageQueue = new UserMessageQueue(RedisProvider);
			var sessionService = new SessionService(RedisProvider);
			var session = await sessionService.Get(data.SessionId);

			if (data.Password != session.CreationData.Password)
			{
				await userMessageQueue.Enqueue(session.Id, b => b.Add(NonMatchingPasswordText).Break().Add(NameInputEvent.EnterPasswordText));
				await sessionService.Update(session.Id, s =>
			   {
				   s.CreationData.Password = null;
				   s.CreationData.ConfirmPasswordInputPending = false;
				   s.CreationData.PasswordInputPending = true;
			   });
				return;
			}

			var model = new UserBuilder(session.CreationData.Name, PasswordValidation.HashPassword(session.CreationData.Password)).Model;

			var user = await _userRepository.Create(model);
			await userMessageQueue.Enqueue(session.Id, b => b.Add("Congrats! You've been created."));
		}
	}
}
