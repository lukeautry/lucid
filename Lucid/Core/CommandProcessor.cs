using System.Threading.Tasks;
using System.Linq;
using Lucid.Commands;
using Lucid.Database;
using Lucid.Events;
using Lucid.Services;

namespace Lucid.Core
{
	public class CommandProcessor
	{
		private readonly IRedisProvider _redisProvider;
		private readonly IUserRepository _userRepository;
		private readonly ISessionUserService _sessionUserService;
		private readonly ISessionService _sessionService;
		private readonly IUserMessageQueue _userMessageQueue;

		public CommandProcessor(
			IRedisProvider redisProvider,
			IUserRepository userRepository,
			ISessionUserService sessionUserService,
			ISessionService sessionService,
			IUserMessageQueue userMessageQueue
			)
		{
			_redisProvider = redisProvider;
			_userRepository = userRepository;
			_sessionUserService = sessionUserService;
			_sessionService = sessionService;
			_userMessageQueue = userMessageQueue;
		}

		public async Task Process(string sessionId, string command)
		{
			var session = await _sessionService.Get(sessionId);
			if (session.NameInputPending)
			{
				await new NameInputEvent(_redisProvider, _userRepository, _sessionService, _userMessageQueue).Enqueue(new NameInputEventData(command, sessionId));
				return;
			}

			if (session.LoginData != null)
			{
				var processed = await ProcessLoginFlow(command, session);
				if (processed) { return; }
			}

			if (session.CreationData != null)
			{
				var processed = await ProcessCreationFlow(command, session);
				if (processed) { return; }
			}

			var arguments = command.Split(' ');
			command = arguments[0];

			// this is apparently some generic command that we'll have to look up
			var genericCommand = CommandMap.Find(command);
			if (genericCommand != null)
			{
				arguments = arguments.Skip(1).ToArray();
				await genericCommand.Process(sessionId, arguments);
				return;
			}

			await new CommandUnrecognizedEvent(_redisProvider, _userMessageQueue).Enqueue(new CommandUnrecognizedEventData(sessionId));
		}

		private async Task<bool> ProcessCreationFlow(string command, SessionData session)
		{
			if (session.CreationData.PasswordInputPending)
			{
				await new NewUserPasswordInputEvent(_redisProvider, _userMessageQueue, _sessionService)
					.Enqueue(new NewUserPasswordInputEventData(session.Id, command));
				return true;
			}

			if (session.CreationData.ConfirmPasswordInputPending)
			{
				await new NewUserConfirmPasswordInputEvent(_redisProvider, _userRepository, _sessionUserService, _userMessageQueue, _sessionService)
					.Enqueue(new NewUserConfirmPasswordInputEventData(session.Id, command));
				return true;
			}

			return false;
		}

		private async Task<bool> ProcessLoginFlow(string command, SessionData session)
		{
			if (session.LoginData.PasswordInputPending)
			{
				await new ExistingUserPasswordInputEvent(_redisProvider, _userRepository, _sessionUserService, _userMessageQueue, _sessionService)
					.Enqueue(new ExistingUserPasswordInputEventData(session.Id, session.LoginData.UserId, command));
				return true;
			}

			return false;
		}
	}
}
