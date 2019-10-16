using System.Collections.Generic;
using System.Linq;
using Dapper;
using Rebronx.Server.Services;

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
            var connection = databaseService.GetConnection();

            connection.Execute(
                "UPDATE players SET node = @node WHERE id = @id",
                new {
                    id = player.Id,
                    node
                });
        }

        public List<Player> GetPlayersByPosition(int node)
        {
            var connection = databaseService.GetConnection();

            var players = connection.Query<Player>(
                "SELECT * FROM players WHERE node = @node",
                new {
                    node
                });

            return players.Where(x => socketRepository.IsPlayerOnline(x.Id)).ToList();
        }
    }
}
