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

		public void MoveItem(int playerId, int from, int to)
		{
			databaseService.ExecuteNonQuery(
				@"UPDATE items 
				SET 
					slot = @to
				WHERE 
					player_id = @playerId AND
					slot = @from AND
					(SELECT COUNT(1) FROM items WHERE player_id = @playerId AND slot = @to) = 0",
				new Dictionary<string, object>() {
					{ "playerId", playerId },
					{ "from", from },
					{ "to", to }
				});
		}

		public void SwapItem(int playerId, int item1, int item2)
		{
			databaseService.ExecuteNonQuery(
				@"UPDATE items
				SET
					slot = CASE WHEN slot = @item1 THEN @item2 ELSE @item1 END
				WHERE
					player_id = @playerId",
				new Dictionary<string, object>() {
					{ "playerId", playerId },
					{ "item1", item1 },
					{ "item2", item2 }
				}
			);
		}

		public void AddItem(int playerId, int item, int count, int slot) 
		{
			databaseService.ExecuteNonQuery(
				@"INSERT INTO items (player_id, item_id, count, slot)
				VALUES (@playerId, @item, @count, @slot)",
				new Dictionary<string, object>() {
					{ "playerId", playerId },
					{ "item", item },
					{ "count", count },
					{ "slot", slot }
				});
		}

		private InventoryItem TransformItem(IDataRecord record) 
		{
			return new InventoryItem() {
				Id = record.GetValueOrDefault<int>("item_id"),
				Count = record.GetValueOrDefault<int>("count"),
				Slot = record.GetValueOrDefault<int>("slot")
			};
		}
	}
}