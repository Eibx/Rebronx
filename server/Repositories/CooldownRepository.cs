using System;
using System.Collections.Generic;
using Dapper;
using Rebronx.Server.Services;

namespace Rebronx.Server.Repositories
{
    public class CooldownRepository : ICooldownRepository
    {
        private readonly IDatabaseService _databaseService;

        public CooldownRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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
            var connection = _databaseService.GetConnection();

            return connection.ExecuteScalar<long?>(
                "SELECT time FROM cooldowns WHERE playerId = @playerId AND type = @type",
                new {
                    playerId = player.Id,
                    type
                });
        }

        public void SetAbsoluteCooldown(Player player, string type, long unixtime)
        {
            var connection = _databaseService.GetConnection();

            connection.Execute(
                "INSERT INTO cooldowns (playerId, type, time) values (@id, @type, @time) ON CONFLICT (playerId, time) DO UPDATE SET time = @time;",
                new {
                    id = player.Id,
                    type = type,
                    time = unixtime,
                }
            );
        }
    }
}