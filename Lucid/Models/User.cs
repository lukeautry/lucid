namespace Lucid.Models
{
    public class User : Model
    {
		public string Name { get; set; }
		public string HashedPassword { get; set; }
    }

	public class UserBuilder : ModelBuilder<User>
	{
		public UserBuilder(string name, string hashedPassword) : base(new User())
		{
			Model.Name = name;
			Model.HashedPassword = hashedPassword;
		}
	}
}
