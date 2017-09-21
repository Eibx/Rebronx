using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Inventory.Repositories
{
	public class InventoryRepository : IInventoryRepository
	{
		private readonly IDatabaseService databaseService;
		private readonly IItemRepository itemRepository;

		public InventoryRepository(IDatabaseService databaseService, IItemRepository itemRepository)
		{
			this.databaseService = databaseService;
			this.itemRepository = itemRepository;
		}

		public List<InventoryItem> GetInventory(int playerId)
		{
			var data = databaseService.ExecuteReader(
				"SELECT * FROM items WHERE player_id = @playerId",
				new Dictionary<string, object>() {
					{ "playerId", playerId }
				});

			var output = new List<InventoryItem>();
			while (data.Read()) {
				output.Add(TransformItem(data));
			}

			data.Close();

			return output;
		}

		public void UnequipItem(int playerId, int equipmentSlot, int inventoryIndex)
		{
			databaseService.ExecuteNonQuery(
				@"UPDATE items 
				SET 
					inv_pos = @inventoryIndex,
					equ_pos = NULL
				WHERE 
					player_id = @playerId AND
					equ_pos = @equipmentSlot AND
					(SELECT COUNT(1) FROM items WHERE player_id = @playerId AND inv_pos = @inventoryIndex) = 0",
				new Dictionary<string, object>() {
					{ "playerId", playerId },
					{ "inventoryIndex", inventoryIndex },
					{ "equipmentSlot", equipmentSlot }
				});
		}

		public void EquipItem(int playerId, int inventoryIndex, int equipmentSlot) 
		{
			databaseService.ExecuteNonQuery(
				@"UPDATE items 
				SET 
					inv_pos = NULL,
					equ_pos = @equipmentSlot
				WHERE 
					player_id = @playerId AND
					inv_pos = @inventoryIndex AND
					(SELECT COUNT(1) FROM items WHERE player_id = @playerId AND equ_pos = @equipmentSlot) = 0",
				new Dictionary<string, object>() {
					{ "playerId", playerId },
					{ "inventoryIndex", inventoryIndex },
					{ "equipmentSlot", equipmentSlot }
				});
		}

		public void ReorderInventory(int playerId, int currentIndex, int newIndex) 
		{
			databaseService.ExecuteNonQuery(
				@"UPDATE items 
				SET 
					inv_pos = @newIndex
				WHERE 
					player_id = @playerId AND
					inv_pos = @currentIndex AND
					(SELECT COUNT(1) FROM items WHERE player_id = @playerId AND inv_pos = @newIndex) = 0",
				new Dictionary<string, object>() {
					{ "playerId", playerId },
					{ "currentIndex", currentIndex },
					{ "newIndex", newIndex }
				});
		}

		private InventoryItem TransformItem(IDataRecord record) 
		{
			return new InventoryItem() {
				Id = record.GetValueOrDefault<int>("item_id"),
				Count = record.GetValueOrDefault<int>("count"),
				InventoryPosition = record.GetValueOrDefault<int?>("inv_pos"),
				EquipmentPosition = record.GetValueOrDefault<int?>("equ_pos"),
			};
		}
	}
}