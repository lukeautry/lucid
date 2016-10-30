using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Events;
using Xunit;

namespace Lucid.Tests.Events
{
    public class ConnectEventTest
    {
	    [Fact]
	    public async Task ShouldInitializeLogin()
	    {
		    var session = new SessionData("test-session");

			var redisProvider = new TestRedisRepository();
		    await new SessionService(redisProvider).Save(session);

		    var evt = new ConnectEvent(redisProvider);
		    await evt.Execute(new ConnectEventData(session.Id));

		    var welcomeMessage = redisProvider.DequeueUserMessage(session.Id);
		    Assert.Contains(ConnectEvent.WelcomeMessage, welcomeMessage.Content);

		    var nameInputMessage = redisProvider.DequeueUserMessage(session.Id);
		    Assert.Contains(ConnectEvent.NameInputMessage, nameInputMessage.Content);

		    var updatedSession = await new SessionService(redisProvider).Get(session.Id);
		    Assert.True(updatedSession.NameInputPending);
	    }
    }
}
