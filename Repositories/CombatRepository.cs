using System;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Repositories
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
			var database = databaseService.GetDatabase();

			var acc = (int?)database.HashGet($"player:{playerId}", "accuracy");
			var agt = (int?)database.HashGet($"player:{playerId}", "agility");

			return new CombatStats() {
				Accuracy = acc.HasValue ? acc.Value : 100,
				Agility = agt.HasValue ? acc.Value : 100
			};
        }
    }
}