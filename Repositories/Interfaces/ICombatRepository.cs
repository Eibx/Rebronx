using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories.Interfaces
{
    public interface ICombatRepository
    {
         CombatStats GetCombatStats(int playerId);
    }
}