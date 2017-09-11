using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Components.Inventory.Repositories
{
	public interface IInventoryRepository
	{
		List<InventoryItem> GetInventory(int playerId);
		void ReorderInventory(int playerId, int currentIndex, int newIndex);
	}
}