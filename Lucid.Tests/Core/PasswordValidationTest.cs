using Lucid.Core;
using Xunit;

namespace Lucid.Tests.Core
{
    public class PasswordValidationTest
    {
		[Theory]
		[InlineData("", false)]
		[InlineData("12", false)]
		[InlineData("Txh6SAKdD7NBlsz0lTi84g7KQSjlj9XwomX2VAOzUjL2bmoktJlhtADUw3aGPV4Naskk3d", false)]
		[InlineData("test1234", true)]
		public static void ValidatesPassword(string password, bool isValid)
		{
			var validationResult = PasswordValidation.ValidatePassword(password);
			Assert.Equal(isValid, validationResult.IsValid);
		}

		[Fact]
	    public static void HashesAndVerifiesPassword()
	    {
		    const string password = "test1234";
		    var hashedPassword = PasswordValidation.HashPassword(password);

		    Assert.True(PasswordValidation.VerifyPassword(password, hashedPassword));
	    }
	}
}
