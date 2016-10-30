﻿using System.Threading.Tasks;
using Lucid.Tests.Fixtures;
using Xunit;
using Lucid.Core;
using Lucid.Events;

namespace Lucid.Tests.Events
{
    public class NewUserConfirmPasswordInputEventTest : IClassFixture<Context>
    {
	    private const string Password = "test1234";
	    private readonly Context _context;

	    public NewUserConfirmPasswordInputEventTest(Context context)
	    {
			_context = context;
			SetState();
		}

		[Fact]
	    public async Task RejectsNonMatchingPassword()
		{
			var session = await _context.GetSession();

		    var evt = new NewUserConfirmPasswordInputEvent(_context.RedisProvider);
		    await evt.Execute(new NewUserConfirmPasswordInputEventData(session.Id, "non-matching-test1234"));

		    var nextMessage = _context.GetNextMessage();
		    Assert.Contains(NewUserConfirmPasswordInputEvent.NonMatchingPasswordText, nextMessage);

			// Dumps them back into the initial enter password flow
			var updatedSession = await _context.GetSession();
			Assert.True(string.IsNullOrEmpty(updatedSession.CreationData.Password));
			Assert.True(updatedSession.CreationData.PasswordInputPending);
			Assert.False(updatedSession.CreationData.ConfirmPasswordInputPending);
			
			Assert.Contains(NameInputEvent.EnterPasswordText, nextMessage);
		}

	    private void SetState()
	    {
		    _context.SetSession(s =>
			{
				s.CreationData = new CreationData
				{
					ConfirmPasswordInputPending = true,
					Password = Password
				};
			});
		}
    }
}