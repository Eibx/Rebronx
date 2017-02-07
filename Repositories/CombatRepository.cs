using System;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Repositories
{
    public class CombatRepository : ICombatRepository
    {
        public CombatStats GetCombatStats(int playerId)
        {
			return new CombatStats() {
				Agility = 100,
				Accuracy = 100
			};
        }
    }
}