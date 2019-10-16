using System.Collections.Generic;

namespace Rebronx.Server.Repositories
{
    public interface IPositionRepository
    {
        void SetPlayerPosition(Player player, int node);
        List<Player> GetPlayersByPosition(int node);
    }
}