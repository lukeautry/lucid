namespace Lucid.Models
{
	public class EquipmentSlot : Model
	{
		public int ItemDefinitionId { get; set; }
		public virtual ItemDefinition ItemDefinition { get; set; }

		public EquipmentType EquipmentType { get; set; }
	}

	public enum EquipmentType
	{
		PrimaryWeapon = 1,
		SecondaryWeapon = 2,
		Held = 3,
		EarOne = 4,
		EarTwo = 5,
		Floating = 6,
		Body = 7,
		Back = 8,
		NeckOne = 9,
		NeckTwo = 10,
		Head = 11,
		Eyes = 12,
		Waist = 13,
		Arms = 14,
		Legs = 15,
		WristOne = 16,
		WristTwo = 17,
		Neck = 18,
		Chest = 19,
		Shoulders = 20
	}
}