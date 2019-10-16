using System;
using Rebronx.Server.Models;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Combat.Repositories
{
    public class CombatRepository : ICombatRepository
    {
        private readonly IDatabaseService databaseService;

        public CombatRepository(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public CombatStats GetCombatStats(int playerId)
        {
            //TODO: use database
            //var database = databaseService.GetConnection();

            int? acc = 100;
            int? agt = 100;

            return new CombatStats()
            {
                Accuracy = acc.HasValue ? acc.Value : 100,
                Agility = agt.HasValue ? acc.Value : 100
            };
        }
    }
}