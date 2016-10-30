namespace Lucid.Core
{
    public static class PasswordValidation
    {
	    public const string PasswordRequiredText = "Password is required.";
	    private const int MinimumPasswordLength = 6;
	    private const int MaximumPasswordLength = 64;


		public static string HashPassword(string password)
	    {
		    return BCrypt.Net.BCrypt.HashPassword(password);
	    }

	    public static bool VerifyPassword(string password, string hashedPassword)
	    {
		    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
	    }

	    public static PasswordValidationResponse ValidatePassword(string password)
	    {
		    if (string.IsNullOrWhiteSpace(password))
		    {
			    return new PasswordValidationResponse(false, PasswordRequiredText);
		    }

		    if (password.Length < MinimumPasswordLength)
		    {
			    return new PasswordValidationResponse(false, PasswordMinimumLengthText());
		    }

			if (password.Length > MaximumPasswordLength)
			{
				return new PasswordValidationResponse(false, PasswordMaximumLengthText());
			}

		    return new PasswordValidationResponse(true, string.Empty);
	    }

	    public static string PasswordMinimumLengthText()
	    {
		    return $"Password should be at least {MinimumPasswordLength} characters.";
	    }

		public static string PasswordMaximumLengthText()
		{
			return $"Password should be no more than {MaximumPasswordLength} characters.";
		}
	}

	public class PasswordValidationResponse
	{
		public readonly bool IsValid;
		public readonly string Message;

		public PasswordValidationResponse(bool isValid, string message)
		{
			IsValid = isValid;
			Message = message;
		}
	}
}
