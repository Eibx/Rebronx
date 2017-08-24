using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using Rebronx.Server.Helpers;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

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
			databaseService.ExecuteNonQuery(
				"INSERT INTO players (name, hash, token) VALUES (@name, @hash, @token)", 
				new Dictionary<string, object> () {
					{ "name", name },
					{ "position", 1 },
					{ "hash", hash },
					{ "token", token }
				});
		}

		public void RemovePlayer(int playerId) 
		{
			databaseService.ExecuteNonQuery(
				"DELETE FROM players WHERE id = @id", 
				new Dictionary<string, object> () {
					{ "id", playerId }
				});
		}

		public Player GetPlayerByName(string name)
		{
			var data = databaseService.ExecuteReader(
				"SELECT * FROM players WHERE name = @name",
				new Dictionary<string, object>() {
					{ "name", name }
				});

			if (!data.Read()) {
				return null;
			}

			var output = TransformPlayer(data);

			return output;
		}

		public Player GetPlayerByLogin(string name, string password)
		{
			var data = databaseService.ExecuteReader(
				"SELECT id, hash FROM players WHERE name = @name",
				new Dictionary<string, object>() {
					{ "name", name }
				});

			if (!data.Read()) {
				return null;
			}				

			var playerId = data.GetInt32(0);
			var playerHash = data.GetString(1);

			if (!PBKDF2.ValidatePassword(password, playerHash))
				return null;
				
			var output = GetPlayerById(playerId);

			return output;
		}

		public Player GetPlayerByToken(string token)
		{
			var data = databaseService.ExecuteReader(
				"SELECT Id FROM players WHERE token = @token",
				new Dictionary<string, object>() {
					{ "token", token }
				});
			
			if (!data.Read()) {
				return null;
			}

			var output = GetPlayerById(data.GetInt32(0));

			return output;
		}

		public Player GetPlayerById(int playerId) 
		{
			var data = databaseService.ExecuteReader(
				"SELECT * FROM players WHERE id = @id",
				new Dictionary<string, object>() {
					{ "id", playerId }
				});

			if (!data.Read()) {
				return null;
			}

			var output = TransformPlayer(data);

			return output;
		}

		private Player TransformPlayer(IDataRecord record) {
			return new Player() {
				Id = record.GetInt32(record.GetOrdinal("id")),
				Name = record.GetString(record.GetOrdinal("name")),
				Position = record.GetInt32(record.GetOrdinal("position")),
				Health = record.GetInt32(record.GetOrdinal("health")),
			};
		}
	}
}
