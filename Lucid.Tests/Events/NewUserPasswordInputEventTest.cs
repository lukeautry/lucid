using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Events;
using Lucid.Tests.Fixtures;
using Xunit;

namespace Lucid.Tests.Events
{
    public class NewUserPasswordInputEventTest : IClassFixture<Context>
    {
	    private readonly Context _context;

		public NewUserPasswordInputEventTest(Context context)
	    {
			_context = context;
	    }

	    [Fact]
	    public async Task ShouldRejectInvalidPassword()
	    {
		    var evt = new NewUserPasswordInputEvent(_context.RedisProvider, new UserMessageQueue(_context.RedisProvider), new SessionService(_context.RedisProvider));
		    await evt.Execute(new NewUserPasswordInputEventData(Context.SessionId, string.Empty));

			var validationMessage = _context.RedisProvider.DequeueUserMessage(Context.SessionId);
			Assert.Contains(PasswordValidation.PasswordRequiredText, validationMessage.Content);
		}

		[Fact]
		public async Task ShouldProcessPassword()
		{
			const string password = "test1234";

			_context.SetSession(s =>
			{
				s.CreationData = new CreationData
				{
					PasswordInputPending = true,
					ConfirmPasswordInputPending = false
				};
			});

			var evt = new NewUserPasswordInputEvent(_context.RedisProvider, new UserMessageQueue(_context.RedisProvider), new SessionService(_context.RedisProvider));
			await evt.Execute(new NewUserPasswordInputEventData(Context.SessionId, password));

			var confirmPasswordMessage = _context.RedisProvider.DequeueUserMessage(Context.SessionId);
			Assert.Contains(NewUserPasswordInputEvent.ConfirmPasswordText, confirmPasswordMessage.Content);

			var session = await _context.GetSession();
			Assert.Equal(password, session.CreationData.Password);
			Assert.False(session.CreationData.PasswordInputPending);
			Assert.True(session.CreationData.ConfirmPasswordInputPending);
		}
	}
}
