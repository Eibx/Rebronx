using System;
using System.Collections.Generic;
using Dapper;
using Rebronx.Server.Services;

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
            var connection = databaseService.GetConnection();
            var token = connection.ExecuteScalar<string>(
                "SELECT token FROM players WHERE id = @id",
                new {
                    id = player.Id
                });

            return token;
        }

        public void SetPlayerToken(Player player, string token)
        {
            var connection = databaseService.GetConnection();
            connection.Execute(
                "UPDATE players SET token = @token WHERE id = @id",
                new {
                    id = player.Id,
                    token = token
                });
        }

        public void RemovePlayerToken(Player player)
        {
            var connection = databaseService.GetConnection();
            connection.Execute(
                "UPDATE players SET token = NULL WHERE id = @id",
                new {
                    id = player.Id
                });
        }
    }
}