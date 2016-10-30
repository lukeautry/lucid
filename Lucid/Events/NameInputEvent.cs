using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;

namespace Lucid.Events
{
	public class NameInputEvent : Event<NameInputEventData>
	{
		private readonly IUserRepository _userRepository;
		private readonly UserMessageQueue _userMessageQueue;
		public const string MaxLengthText = "Name must be no more than 64 characters.";
		public const string MinLengthText = "Name should be at least two characters.";
		public const string NameRequiredText = "Name is required.";
		public const string AlphaOnlyText = "Name must be letters only.";
		public const string EnterPasswordText = "Please enter your password:";

		public NameInputEvent(IUserRepository userRepository = null, IRedisProvider redisProvider = null) : base("name-input", redisProvider)
		{
			_userRepository = userRepository ?? new UserRepository();
			_userMessageQueue = new UserMessageQueue(RedisProvider);
		}

		public override async Task Execute(NameInputEventData data)
		{
			if (!await IsValidName(data)) { return; }

			var sessionService = new SessionService(RedisProvider);
			var user = await _userRepository.GetByName(data.Name);
			if (user != null)
			{
				await ProcessExistingUser(data, sessionService, user.Id);
				return;
			}
			
			await ProcessNewUser(data, sessionService);
		}

		private async Task<bool> IsValidName(NameInputEventData data)
		{
			if (string.IsNullOrWhiteSpace(data.Name))
			{
				return await EmitValidationMessage(data, NameRequiredText);
			}

			if (data.Name.Length < 2)
			{
				return await EmitValidationMessage(data, MinLengthText);
			}

			if (data.Name.Length > 64)
			{
				return await EmitValidationMessage(data, MaxLengthText);
			}

			if (Regex.IsMatch(data.Name, @"^[a-zA-Z]+$")) return true;
			{
				return await EmitValidationMessage(data, AlphaOnlyText);
			}
		}

		private async Task<bool> EmitValidationMessage(NameInputEventData data, string message)
		{
			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add(message));
			return false;
		}

		private async Task ProcessExistingUser(NameInputEventData data, SessionService sessionService, int userId)
		{
			await sessionService.Update(data.SessionId, s =>
			{
				s.NameInputPending = false;
				s.LoginData = new LoginData { UserId = userId, PasswordInputPending = true };
			});

			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add($"Welcome back {data.Name}!").Break().Add(EnterPasswordText));
		}

		private async Task ProcessNewUser(NameInputEventData data, SessionService sessionService)
		{
			await sessionService.Update(data.SessionId, s =>
			{
				s.NameInputPending = false;
				s.CreationData = new CreationData
				{
					Name = data.Name,
					PasswordInputPending = true,
					ConfirmPasswordInputPending = true
				};
			});

			await _userMessageQueue.Enqueue(data.SessionId, b => b.Add($"Welcome {data.Name}! Looks like you're new here.").Break().Add(EnterPasswordText));
		}
	}

	public class NameInputEventData
	{
		public readonly string Name;
		public readonly string SessionId;

		public NameInputEventData(string name, string sessionId)
		{
			Name = name;
			SessionId = sessionId;
		}
	}
}
