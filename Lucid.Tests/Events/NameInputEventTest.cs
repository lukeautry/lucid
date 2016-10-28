using System.Threading.Tasks;
using Lucid.Database;
using Lucid.Events;
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
    }
}
