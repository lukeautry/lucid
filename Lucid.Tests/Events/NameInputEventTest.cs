using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Events;
using Lucid.Models;
using Moq;
using Xunit;

namespace Lucid.Tests.Events
{
    public class NameInputEventTest
    {
	    [Theory]
		[InlineData("", NameInputEvent.NameRequiredText)]
		[InlineData("1234", NameInputEvent.AlphaOnlyText)]
		[InlineData("X", NameInputEvent.MinLengthText)]
		[InlineData("Txh6SAKdD7NBlsz0lTi84g7KQSjlj9XwomX2VAOzUjL2bmoktJlhtADUw3aGPV4Na", NameInputEvent.MaxLengthText)]
		public async Task ValidatesName(string name, string expectedErrorText)
	    {
			const string sessionId = "test-session";
			var userRepository = new Mock<IUserRepository>();
			var redisProvider = new TestRedisRepository();

			var evt = new NameInputEvent(userRepository.Object, redisProvider);
			await evt.Execute(new NameInputEventData(name, sessionId));

			var userMessage = redisProvider.DequeueUserMessage(sessionId);
			Assert.Contains(expectedErrorText, userMessage.Content);
		}

		[Fact]
	    public async Task ProcessesExistingUser()
	    {
			var session = new SessionData("test-session");

			var redisProvider = new TestRedisRepository();
			await new SessionService(redisProvider).Save(session);
			
		    const string userName = "TestName";
			
			var userRepository = new Mock<IUserRepository>();
		    userRepository.Setup(u => u.GetByName(userName)).ReturnsAsync(new User { Name = userName, Id = 1 });

			var evt = new NameInputEvent(userRepository.Object, redisProvider);
			await evt.Execute(new NameInputEventData(userName, session.Id));

			var updatedSession = await new SessionService(redisProvider).Get(session.Id);
			Assert.True(updatedSession.LoginData.PasswordInputPending);
		    Assert.False(updatedSession.NameInputPending);

			var userMessage = redisProvider.DequeueUserMessage(session.Id);
			Assert.Contains(NameInputEvent.EnterPasswordText, userMessage.Content);
		}

		[Fact]
		public async Task ProcessesNewUser()
		{
			var session = new SessionData("test-session");

			var redisProvider = new TestRedisRepository();
			await new SessionService(redisProvider).Save(session);

			const string userName = "TestName";

			var userRepository = new Mock<IUserRepository>();
			userRepository.Setup(u => u.GetByName(userName)).ReturnsAsync(null);

			var evt = new NameInputEvent(userRepository.Object, redisProvider);
			await evt.Execute(new NameInputEventData(userName, session.Id));

			var updatedSession = await new SessionService(redisProvider).Get(session.Id);
			Assert.Equal(updatedSession.CreationData.Name, userName);
			Assert.True(updatedSession.CreationData.PasswordInputPending);
			Assert.False(updatedSession.NameInputPending);

			var userMessage = redisProvider.DequeueUserMessage(session.Id);
			Assert.Contains(NameInputEvent.EnterPasswordText, userMessage.Content);
		}
	}
}
