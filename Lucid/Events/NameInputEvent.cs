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
		public override string Key { get; set; } = "name-input";
		public const string MaxLengthText = "Name must be no more than 64 characters.";
		public const string MinLengthText = "Name should be at least two characters.";
		public const string NameRequiredText = "Name is required.";
		public const string AlphaOnlyText = "Name must be letters only.";

		public NameInputEvent(IUserRepository userRepository = null, IRedisProvider redisProvider = null) : base(redisProvider)
		{
			_userRepository = userRepository ?? new UserRepository();
			_userMessageQueue = new UserMessageQueue(redisProvider ?? new RedisProvider());
		}

		public override async Task Execute(NameInputEventData data)
		{
			if (!await IsValidName(data)) { return; }

			var user = _userRepository.GetByName(data.Name);
			if (user != null)
			{
				await ProcessExistingUser();
				return;
			}

			//await ProcessNewUser(data);
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

		private async Task ProcessNewUser()
		{

		}

		private async Task ProcessExistingUser()
		{

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
