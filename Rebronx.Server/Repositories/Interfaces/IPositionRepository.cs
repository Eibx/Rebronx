using System.Collections.Generic;

namespace Rebronx.Server.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        void SetPlayerPositon(Player player, Position position);
        List<Player> GetPlayersByPosition(Position position);
    }
}