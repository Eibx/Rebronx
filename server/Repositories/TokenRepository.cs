using System;
using System.Collections.Generic;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IDatabaseService databaseService;

        public TokenRepository(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public string GetToken(Player player)
        {
            var token = databaseService.ExecuteScalar<String>(
                "SELECT token FROM players WHERE id = @id",
                new Dictionary<string, object>() {
                    { "id", player.Id }
                });

            return token;
        }

        public void SetPlayerToken(Player player, string token)
        {
            databaseService.ExecuteNonQuery(
                "UPDATE players SET token = @token WHERE id = @id",
                new Dictionary<string, object>() {
                    { "id", player.Id },
                    { "token", token }
                });
        }

        public void RemovePlayerToken(Player player)
        {
            databaseService.ExecuteNonQuery(
                "UPDATE players SET token = NULL WHERE id = @id",
                new Dictionary<string, object>() {
                    { "id", player.Id }
                });
        }
    }
}