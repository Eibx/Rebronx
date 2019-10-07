using System;
using System.Collections.Generic;
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

            if (unixtime.HasValue)
            {
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
            return databaseService.ExecuteScalar<long?>(
                "SELECT time FROM cooldowns WHERE playerId = @playerId AND type = @type",
                new Dictionary<string, object>() {
                    { "playerId", player.Id },
                    { "type", type }
                });
        }

        public void SetAbsoluteCooldown(Player player, string type, long unixtime)
        {
            databaseService.ExecuteNonQuery(
                "INSERT INTO cooldowns (playerId, type, time) values (@id, @type, @time) ON CONFLICT (playerId, time) DO UPDATE SET time = @time;",
                new Dictionary<string, object>() {
                    { "id", player.Id },
                    { "type", type },
                    { "time", unixtime }
                }
            );
        }
    }
}