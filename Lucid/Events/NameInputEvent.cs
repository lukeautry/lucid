using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;

namespace Lucid.Events
{
	public class NameInputEvent : BlockingEvent<NameInputEventData>
	{
		private readonly IUserRepository _userRepository;
		private readonly UserMessageQueue _userMessageQueue;
		public const string MaxLengthText = "Name must be no more than 64 characters.";
		public const string MinLengthText = "Name should be at least two characters.";
		public const string NameRequiredText = "Name is required.";
		public const string AlphaOnlyText = "Name must be letters only.";
		public const string EnterPasswordText = "Please enter your password:";

		public NameInputEvent(IRedisProvider redisProvider, IUserRepository userRepository) : base("name-input", redisProvider)
		{
			_userRepository = userRepository;
			_userMessageQueue = new UserMessageQueue(RedisProvider);
		}

		protected override async Task ExecuteBlockingEvent(NameInputEventData data)
		{
			if (!await IsValidName(data)) { return; }

			var sessionService = new SessionService(RedisProvider);
			var user = await _userRepository.GetByName(data.Name);
			if (user != null)
			{
				await ProcessExistingUser(data, sessionService, user);
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

		private async Task ProcessExistingUser(NameInputEventData data, SessionService sessionService, User user)
		{
			await sessionService.Update(data.SessionId, s =>
			{
				s.NameInputPending = false;
				s.LoginData = new LoginData { UserId = user.Id, PasswordInputPending = true };
			});

			await _userMessageQueue.Enqueue(data.SessionId, b => b.Break().Add($"Welcome back {user.Name}!").Break().Add(EnterPasswordText));
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

	public class NameInputEventData : BlockingEventData
	{
		public readonly string Name;

		public NameInputEventData(string name, string sessionId) : base(sessionId)
		{
			Name = name;
		}
	}
}
