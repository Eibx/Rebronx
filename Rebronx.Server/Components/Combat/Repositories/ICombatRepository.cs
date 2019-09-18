using Rebronx.Server.Models;

namespace Rebronx.Server.Components.Combat.Repositories
{
    public interface ICombatRepository
    {
        CombatStats GetCombatStats(int playerId);
    }
}