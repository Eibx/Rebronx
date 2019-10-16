using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using Dapper;
using Rebronx.Server.Helpers;
using Rebronx.Server.Models;
using Rebronx.Server.Services;

namespace Rebronx.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseService databaseService;

        public UserRepository(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public void CreateNewPlayer(string name, string hash, string token)
        {
            var connection = databaseService.GetConnection();
            connection.Execute(
                "INSERT INTO players (name, hash, token) VALUES (@name, @hash, @token)",
                new {
                    name,
                    position = 1,
                    hash,
                    token,
                });
        }

        public void RemovePlayer(int playerId)
        {
            var connection = databaseService.GetConnection();
            connection.Execute(
                "DELETE FROM players WHERE id = @id",
                new {
                    id = playerId,
                });
        }

        public Player GetPlayerByName(string name)
        {
            var connection = databaseService.GetConnection();
            var player = connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM players WHERE name = @name",
                new {
                    name,
                });

            return player;
        }

        public Player GetPlayerByLogin(string name, string password)
        {
            var connection = databaseService.GetConnection();
            var data = connection.QueryFirstOrDefault<UserAuthentication>(
                "SELECT id, hash FROM players WHERE name = @name",
                new {
                    name
                });

            if (data == null)
                return null;

            var playerId = data.Id;
            var playerHash = data.Hash;

            if (!PBKDF2.ValidatePassword(password, playerHash))
                return null;

            return GetPlayerById(playerId);
        }

        public Player GetPlayerByToken(string token)
        {
            var connection = databaseService.GetConnection();
            var player = connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM players WHERE token = @token",
                new {
                    token = token,
                });

            return player;
        }

        public Player GetPlayerById(int playerId)
        {
            var connection = databaseService.GetConnection();
            var player = connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM players WHERE id = @id",
                new {
                    id = playerId,
                });

            return player;
        }

        private Player TransformPlayer(IDataRecord record) {
            return new Player() {
                Id = record.GetInt32(record.GetOrdinal("id")),
                Name = record.GetString(record.GetOrdinal("name")),
                Node = record.GetInt32(record.GetOrdinal("node")),
                Health = record.GetInt32(record.GetOrdinal("health")),
            };
        }
    }
}
