using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Components.Inventory.Repositories
{
	public interface IInventoryRepository
	{
		List<InventoryItem> GetInventory(int playerId);
		void UnequipItem(int playerId, int equipmentSlot, int inventoryIndex);
		void EquipItem(int playerId, int inventoryIndex, int equipmentSlot);
		void ReorderInventory(int playerId, int currentIndex, int newIndex);
	}
}