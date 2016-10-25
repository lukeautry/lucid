using System;
using System.Threading.Tasks;

namespace Lucid.Core
{
    public sealed class CommandProcessor
    {
	    public async Task Process(string sessionId, string command)
	    {
		    await new UserMessageQueue(sessionId).Enqueue(new UserMessageData
		    {
			    Content = $"Alright, looks like we got the command {command}"
			});
	    }
    }
}
