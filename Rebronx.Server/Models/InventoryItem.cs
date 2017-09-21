namespace Rebronx.Server.Models
{
	public class InventoryItem
	{
		public int Id { get; set; }
		public int Count { get; set; }
		public int? InventoryPosition { get; set; }
		public int? EquipmentPosition { get; set; }
	}
}