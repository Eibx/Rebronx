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

		private InventoryItem TransformItem(IDataRecord record) 
		{
			return new InventoryItem() {
				Id = record.GetInt32(record.GetOrdinal("item_id")),
				Position = record.GetInt32(record.GetOrdinal("position")),
				Count = record.GetInt32(record.GetOrdinal("count"))
			};
		}
	}
}