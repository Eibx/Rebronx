using System;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Repositories
{
    public class CooldownRepository : ICooldownRepository
    {
		private readonly IDatabaseService databaseService;

		public CooldownRepository(IDatabaseService databaseService)
		{
			this.databaseService = databaseService;
		}

        public long? GetCooldown(Player player, string type)
        {
			var unixtime = GetAbsoluteCooldown(player, type);

			if (unixtime.HasValue) {
				unixtime -= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			}

			return unixtime;
        }

		public void SetCooldown(Player player, string type, long miliseconds) 
		{
			SetAbsoluteCooldown(player, type, miliseconds + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
		}

		public long? GetAbsoluteCooldown(Player player, string type)
        {
			var database = databaseService.GetDatabase();

			var unixtime = (long?)database.HashGet($"cooldown:{player.Id}", type);

			long? output = null;

			if (unixtime.HasValue) {
				output = unixtime.Value;
			}

			return output;
        }

		public void SetAbsoluteCooldown(Player player, string type, long unixtime) 
		{
			var database = databaseService.GetDatabase();
			database.HashSet($"cooldown:{player.Id}", type, unixtime);
		}
    }
}