using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Events;
using Lucid.Tests.Fixtures;
using Xunit;

namespace Lucid.Tests.Events
{
    public class NewUserPasswordInputEventTest : IClassFixture<Context>
    {
	    public readonly Context Context;

		public NewUserPasswordInputEventTest(Context context)
	    {
			Context = context;
	    }

	    [Fact]
	    public async Task ShouldRejectInvalidPassword()
	    {
		    var evt = new NewUserPasswordInputEvent(Context.RedisProvider);
		    await evt.Execute(new NewUserPasswordInputEventData(Context.SessionId, string.Empty));

			var validationMessage = Context.RedisProvider.DequeueUserMessage(Context.SessionId);
			Assert.Contains(PasswordValidation.PasswordRequiredText, validationMessage.Content);
		}

		[Fact]
		public async Task ShouldProcessPassword()
		{
			const string password = "test1234";

			Context.SetSession(s =>
			{
				s.CreationData = new CreationData
				{
					PasswordInputPending = true,
					ConfirmPasswordInputPending = false
				};
			});

			var evt = new NewUserPasswordInputEvent(Context.RedisProvider);
			await evt.Execute(new NewUserPasswordInputEventData(Context.SessionId, password));

			var confirmPasswordMessage = Context.RedisProvider.DequeueUserMessage(Context.SessionId);
			Assert.Contains(NewUserPasswordInputEvent.ConfirmPasswordText, confirmPasswordMessage.Content);

			var session = await Context.GetSession();
			Assert.Equal(password, session.CreationData.Password);
			Assert.False(session.CreationData.PasswordInputPending);
			Assert.True(session.CreationData.ConfirmPasswordInputPending);
		}
	}
}
