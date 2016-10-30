namespace Lucid.Models
{
    public class User : Model
    {
		public string Name { get; set; }
		public string HashedPassword { get; set; }
    }
}
