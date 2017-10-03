using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Components.Inventory.Repositories
{
	public interface IInventoryRepository
	{
		List<InventoryItem> GetInventory(int playerId);
		void MoveItem(int playerId, int from, int to);
		void SwapItem(int playerId, int item1, int item2);
		void AddItem(int playerId, int item, int count, int slot);
	}
}