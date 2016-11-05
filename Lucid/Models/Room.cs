using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lucid.Models
{
	public class Room : Model
	{
		[Required]
		[MaxLength(256)]
		public string Name { get; set; }

		[MaxLength(2048)]
		public string Description { get; set; }

		[Required]
		public int AreaId { get; set; }
		public Area Area { get; set; }
		
		public Room NorthRoom { get; set; }
		public int? NorthRoomId { get; set; }
		
		public Room EastRoom { get; set; }
		public int? EastRoomId { get; set; }
		
		public Room SouthRoom { get; set; }
		public int? SouthRoomId { get; set; }
		
		public Room WestRoom { get; set; }
		public int? WestRoomId { get; set; }
		
		public Room UpRoom { get; set; }
		public int? UpRoomId { get; set; }
		
		public Room DownRoom { get; set; }
		public int? DownRoomId { get; set; }
	}

	public class RoomCreationRequest
	{
		[Required]
		public int AreaId { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }
		public int? NorthRoomId { get; set; }
		public int? EastRoomId { get; set; }
		public int? SouthRoomId { get; set; }
		public int? WestRoomId { get; set; }
		public int? UpRoomId { get; set; }
		public int? DownRoomId { get; set; }
	}

	public class RoomUpdateRequest
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public int AreaId { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }
		public int? NorthRoomId { get; set; }
		public int? EastRoomId { get; set; }
		public int? SouthRoomId { get; set; }
		public int? WestRoomId { get; set; }
		public int? UpRoomId { get; set; }
		public int? DownRoomId { get; set; }
	}

	public class RoomBuilder : ModelBuilder<Room>
	{
		public RoomBuilder(string name, string description, int areaId, int? northRoomId, int? eastRoomId, int? southRoomId, int? westRoomId, int? upRoomId, int? downRoomId) : base(new Room())
		{
			Model.Name = name;
			Model.Description = description;
			Model.AreaId = areaId;
			Model.NorthRoomId = northRoomId;
			Model.EastRoomId = eastRoomId;
			Model.SouthRoomId = southRoomId;
			Model.WestRoomId = westRoomId;
			Model.UpRoomId = upRoomId;
			Model.DownRoomId = downRoomId;
		}
	}
}
