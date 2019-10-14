using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IDatabaseService databaseService;
        private readonly ISocketRepository socketRepository;

        public PositionRepository(IDatabaseService databaseService, ISocketRepository socketRepository)
        {
            this.databaseService = databaseService;
            this.socketRepository = socketRepository;
        }

        public void SetPlayerPosition(Player player, int node)
        {
            databaseService.ExecuteNonQuery(
                "UPDATE players SET node = @node WHERE id = @id",
                new Dictionary<string, object>() {
                    { "id", player.Id },
                    { "node", node }
                });
        }

        public List<Player> GetPlayersByPosition(int node)
        {
            var data = databaseService.ExecuteReader(
                "SELECT * FROM players WHERE node = @node",
                new Dictionary<string, object>() {
                    { "node", node }
                });

            var output = new List<Player>();
            while (data.Read()) {
                output.Add(TransformPlayer(data));
            }

            data.Close();

            return output.Where(x => socketRepository.IsPlayerOnline(x.Id)).ToList();
        }

        private Player TransformPlayer(IDataRecord record) {
            return new Player() {
                Id = record.GetInt32(record.GetOrdinal("id")),
                Name = record.GetString(record.GetOrdinal("name")),
                Node = record.GetInt32(record.GetOrdinal("node")),
                Health = record.GetInt32(record.GetOrdinal("health")),
            };
        }
    }
}
