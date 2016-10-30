using System.Threading.Tasks;
using Lucid.Events;

namespace Lucid.Core
{
    public static class CommandProcessor
    {
	    public static async Task Process(string sessionId, string command)
	    {
		    var session = await new SessionService().Get(sessionId);
		    if (session.NameInputPending)
		    {
			    await new NameInputEvent().Enqueue(new NameInputEventData(command, sessionId));
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
	    }

	    private static async Task<bool> ProcessCreationFlow(string command, SessionData session)
	    {
		    if (session.CreationData.PasswordInputPending)
		    {
			    await new NewUserPasswordInputEvent().Enqueue(new NewUserPasswordInputEventData(session.Id, command));
				return true;
		    }

			return false;
	    }

	    private static async Task<bool> ProcessLoginFlow(string command, SessionData session)
	    {
		    if (session.LoginData.PasswordInputPending)
		    {
			    await new ExistingUserPasswordInputEvent().Enqueue(new ExistingUserPasswordInputEventData(session.Id, session.LoginData.UserId, command));
			    return true;
		    }

			return false;
	    }
    }
}
