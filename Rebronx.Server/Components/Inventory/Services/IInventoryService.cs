namespace Rebronx.Server.Components.Inventory.Services
{
	public interface IInventoryService
	{
		void MoveItem(int playerId, int from, int? to);
	}
}