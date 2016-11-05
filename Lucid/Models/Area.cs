using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lucid.Models
{
	public class Area : Model
	{
		[Required]
		[MaxLength(64)]
		public string Name { get; set; }

		[MaxLength(1024)]
		public string Description { get; set; }

		public ICollection<Room> Rooms { get; set; }
	}

	public class AreaCreationRequest
	{
		[Required]
		public string Name { get; set; }

		public string Description { get; set; }
	}

	public class AreaUpdateRequest
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }
	}

	public class AreaBuilder : ModelBuilder<Area>
	{
		public AreaBuilder(string name, string description) : base(new Area())
		{
			Model.Name = name;
			Model.Description = description;
		}
	}
}
