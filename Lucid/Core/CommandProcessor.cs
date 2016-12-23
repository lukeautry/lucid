using System;
using System.Threading.Tasks;
using Lucid.Commands;
using Lucid.Database;
using Lucid.Events;

namespace Lucid.Core
{
    public class CommandProcessor
    {
	    private readonly IRedisProvider _redisProvider;
	    private readonly IServiceProvider _serviceProvider;
	    private readonly IUserRepository _userRepository;
	    private readonly IRoomRepository _roomRepository;

	    public CommandProcessor(IRedisProvider redisProvider, IServiceProvider serviceProvider, IUserRepository userRepository, IRoomRepository roomRepository)
	    {
		    _redisProvider = redisProvider;
		    _serviceProvider = serviceProvider;
		    _userRepository = userRepository;
			_roomRepository = roomRepository;
	    }

	    public async Task Process(string sessionId, string command)
	    {
			var session = await new SessionService(_redisProvider).Get(sessionId);
			if (session.NameInputPending)
			{
				await new NameInputEvent(_redisProvider, _userRepository).Enqueue(new NameInputEventData(command, sessionId));
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

			// this is apparently some generic command that we'll have to look up
		    var genericCommand = new CommandMap(_serviceProvider).Find(command);
		    if (genericCommand != null)
		    {
			    await genericCommand.Process(sessionId);
				return;
		    }

			await new CommandUnrecognizedEvent(_redisProvider).Enqueue(new CommandUnrecognizedEventData(sessionId));
	    }

	    private async Task<bool> ProcessCreationFlow(string command, SessionData session)
	    {
		    if (session.CreationData.PasswordInputPending)
		    {
			    await new NewUserPasswordInputEvent(_redisProvider).Enqueue(new NewUserPasswordInputEventData(session.Id, command));
				return true;
		    }

		    if (session.CreationData.ConfirmPasswordInputPending)
		    {
			    await new NewUserConfirmPasswordInputEvent(_redisProvider, _userRepository).Enqueue(new NewUserConfirmPasswordInputEventData(session.Id, command));
				return true;
		    }

			return false;
	    }

	    private async Task<bool> ProcessLoginFlow(string command, SessionData session)
	    {
		    if (session.LoginData.PasswordInputPending)
		    {
			    await new ExistingUserPasswordInputEvent(_redisProvider, _userRepository, _roomRepository).Enqueue(new ExistingUserPasswordInputEventData(session.Id, session.LoginData.UserId, command));
			    return true;
		    }

			return false;
	    }
	}
}
