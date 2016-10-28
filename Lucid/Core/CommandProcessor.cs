using System.Threading.Tasks;

namespace Lucid.Core
{
    public sealed class CommandProcessor
    {
	    public async Task Process(string sessionId, string command)
	    {
		    var session = await new Session().Get(sessionId);
		    if (session.NameInputPending)
		    {
			    
		    }
	    }
    }
}
