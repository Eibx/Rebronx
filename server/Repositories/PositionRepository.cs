using System.Collections.Generic;
using System.Linq;
using Dapper;
using Rebronx.Server.Services;

namespace Rebronx.Server.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IDatabaseService _databaseService;
        private readonly ISocketRepository _socketRepository;

        public PositionRepository(IDatabaseService databaseService, ISocketRepository socketRepository, IUserRepository userRepository)
        {
            _databaseService = databaseService;
            _socketRepository = socketRepository;
        }

        public void SetPlayerPosition(Player player, int node)
        {
            var connection = _databaseService.GetConnection();

            connection.Execute(
                "UPDATE players SET node = @node WHERE id = @id",
                new {
                    id = player.Id,
                    node
                });
            connection.Close();
        }

        public List<Player> GetPlayersByPosition(int node)
        {
            var connection = _databaseService.GetConnection();

            var players = connection.Query<Player>(
                "SELECT * FROM players WHERE node = @node",
                new {
                    node
                });
            connection.Close();

            return players.Where(x => _socketRepository.IsPlayerOnline(x.Id)).ToList();
        }
    }
}
