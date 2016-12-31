using System.ComponentModel.DataAnnotations;

namespace Lucid.Models
{
	public sealed class ItemDefinition : Model
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }
	}

	public sealed class ItemDefinitionCreationRequest
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }
	}

	public sealed class ItemDefinitionUpdateRequest
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }
	}

	public sealed class ItemDefinitionBuilder : ModelBuilder<ItemDefinition>
	{
		public ItemDefinitionBuilder(string name, string description) : base(new ItemDefinition())
		{
			Model.Name = name;
			Model.Description = description;
		}
	}
}
