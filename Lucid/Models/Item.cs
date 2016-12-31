using System.ComponentModel.DataAnnotations;

namespace Lucid.Models
{
	public class Item : Model
	{
		[Required]
		public int ParentObjectId { get; set; }

		[Required]
		public ObjectType ParentObjectType { get; set; }

		[Required]
		public int ItemDefinitionId { get; set; }
		public virtual ItemDefinition ItemDefinition { get; set; }
	}

	public class ItemCreationRequest
	{
		[Required]
		public int ParentObjectId { get; set; }

		[Required]
		public ObjectType ParentObjectType { get; set; }

		[Required]
		public int ItemDefinitionId { get; set; }
	}

	public enum ObjectType
	{
		Room = 1,
		User = 2
	}

	public class ItemBuilder : ModelBuilder<Item>
	{
		public ItemBuilder(int parentObjectId, ObjectType parentObjectType, int itemDefinitionId) : base(new Item())
		{
			Model.ParentObjectId = parentObjectId;
			Model.ParentObjectType = parentObjectType;
			Model.ItemDefinitionId = itemDefinitionId;
		}
	}
}
