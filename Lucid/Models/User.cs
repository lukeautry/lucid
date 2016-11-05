using System.ComponentModel.DataAnnotations;

namespace Lucid.Models
{
    public class User : Model
    {
		[Required]
		[MaxLength(64)]
		public string Name { get; set; }

		[Required]
		public string HashedPassword { get; set; }

		public int? CurrentRoomId { get; set; }
		public Room CurrentRoom { get; set; }
    }

	public class UserUpdateRequest
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public int CurrentRoomId { get; set; }
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
