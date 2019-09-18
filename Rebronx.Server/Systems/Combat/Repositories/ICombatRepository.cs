using Rebronx.Server.Models;

namespace Rebronx.Server.Systems.Combat.Repositories
{
    public interface ICombatRepository
    {
        CombatStats GetCombatStats(int playerId);
    }
}