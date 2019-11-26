using System;
using System.Collections.Generic;
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
        private readonly IDatabaseService _databaseService;

        public UserRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void CreateNewPlayer(string name, string hash, string token)
        {
            var connection = _databaseService.GetConnection();
            connection.Execute(
                "INSERT INTO players (name, hash, token, node) VALUES (@name, @hash, @token, @node)",
                new {
                    name,
                    hash,
                    token,
                    node = 1,
                });
            connection.Close();
        }

        public void RemovePlayer(int playerId)
        {
            var connection = _databaseService.GetConnection();
            connection.Execute(
                "DELETE FROM players WHERE id = @id",
                new {
                    id = playerId,
                });
            connection.Close();
        }

        public Player GetPlayerByName(string name)
        {
            var connection = _databaseService.GetConnection();
            var player = connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM players WHERE name = @name",
                new {
                    name,
                });
            connection.Close();

            return player;
        }

        public Player GetPlayerByLogin(string name, string password)
        {
            var connection = _databaseService.GetConnection();
            var data = connection.QueryFirstOrDefault<UserAuthentication>(
                "SELECT id, hash FROM players WHERE name = @name",
                new {
                    name
                });
            connection.Close();

            if (data == null)
                return null;

            var playerId = data.Id;
            var playerHash = data.Hash;

            if (!Pbkdf2.ValidatePassword(password, playerHash))
                return null;

            var iterations = Pbkdf2.GetHashIterations(playerHash);
            if (iterations != Pbkdf2.Pbkdf2Iterations)
            {
                var hash = Pbkdf2.HashPassword(password);
                UpdateHash(playerId, hash);
            }

            return GetPlayerById(playerId);
        }

        public Player GetPlayerByToken(string token)
        {
            var connection = _databaseService.GetConnection();
            var player = connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM players WHERE token = @token",
                new {
                    token = token,
                });
            connection.Close();

            return player;
        }

        public Player GetPlayerById(int playerId)
        {
            var connection = _databaseService.GetConnection();
            var player = connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM players WHERE id = @id",
                new {
                    id = playerId,
                });
            connection.Close();

            return player;
        }

        public void UpdateHash(int playerId, string hash)
        {
            var connection = _databaseService.GetConnection();
            var player = connection.Execute(
                "UPDATE players SET hash = @hash WHERE id = @id",
                new
                {
                    id = playerId,
                    hash
                });
            connection.Close();
        }
    }
}
