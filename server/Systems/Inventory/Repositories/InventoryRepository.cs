using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Inventory.Repositories
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
            var connection = databaseService.GetConnection();
            var data = connection.ExecuteReader(
                "SELECT * FROM items WHERE player_id = @playerId",
                new {
                    playerId,
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
            var connection = databaseService.GetConnection();
            connection.Execute(
                @"UPDATE items
                SET
                    slot = @to
                WHERE
                    player_id = @playerId AND
                    slot = @from AND
                    (SELECT COUNT(1) FROM items WHERE player_id = @playerId AND slot = @to) = 0",
                new {
                    playerId,
                    from,
                    to,
                });
        }

        public void SwapItem(int playerId, int item1, int item2)
        {
            var connection = databaseService.GetConnection();
            connection.Execute(
                @"UPDATE items
                SET
                    slot = CASE WHEN slot = @item1 THEN @item2 ELSE @item1 END
                WHERE
                    player_id = @playerId",
                new {
                    playerId,
                    item1,
                    item2,
                }
            );
        }

        public void AddItem(int playerId, int item, int count, int slot)
        {
            var connection = databaseService.GetConnection();
            connection.Execute(
                @"INSERT INTO items (player_id, item_id, count, slot)
                VALUES (@playerId, @item, @count, @slot)",
                new {
                    playerId,
                    item,
                    count,
                    slot,
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