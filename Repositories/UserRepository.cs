using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Rebronx.Server.Helpers;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;
using StackExchange.Redis;

namespace Rebronx.Server.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly RedisValue[] playerFields;

		private readonly IDatabaseService databaseService;
		private readonly IDatabase database;
		private readonly ISocketRepository socketRepository;

		public UserRepository(IDatabaseService databaseService, ISocketRepository socketRepository)
		{
			this.databaseService = databaseService;
			this.socketRepository = socketRepository;
			this.database = databaseService.GetDatabase();

			this.playerFields = new RedisValue[] {
				"name",
				"health",
				"pos.x",
				"pos.y",
				"pos.z",
			};
		}

		public void CreateNewPlayer(string username, string hash, string token)
		{
			var database = databaseService.GetDatabase();
			var id = NextUserId();
			
			database.HashSet($"player:{id}", "name", username);
			database.HashSet($"player:{id}", "health", 100);
			database.HashSet($"player:{id}", "credits", 0);
			database.HashSet($"player:{id}", "token", token);
			database.HashSet($"login:{username.ToLower()}", "hash", hash);
			database.HashSet($"login:{username.ToLower()}", "id", id);
			database.StringSet($"token:{token}", id);
			database.SetAdd($"position:0.0.0", id);
		}

		public void RemovePlayer(int playerId) 
		{
			var token = (string)database.HashGet($"player:{playerId}", "token");
			var username = (string)database.HashGet($"player:{playerId}", "name");
			database.KeyDelete($"player:{playerId}");
			database.KeyDelete($"cooldown:{playerId}");
			database.KeyDelete($"login:{username.ToLower()}");
			database.KeyDelete($"token:{token}");
		}

		public Player GetPlayerByUsername(string username)
		{
			Player player = null;
			if (string.IsNullOrEmpty(username)) 
			{
				return player;
			}

			var key = $"login:{username.ToLower()}";
			var playerId = (int?)database.HashGet(key, "id");

			if (playerId.HasValue)
				return GetPlayerById(playerId.Value);

			return player;
		}

		public Player GetPlayerByLogin(string username, string password)
		{
			Player player = null;

			if (string.IsNullOrEmpty(username))
				return player;

			var key = $"login:{username.ToLower()}";
			var hash = database.HashGet(key, "hash");
			
			if (hash == "TEST")
			{
				var newHash = PBKDF2.HashPassword(password);
				database.HashSet(key, "hash", newHash);
			}

			var playerId = (int?)database.HashGet(key, "id");

			if (playerId.HasValue && PBKDF2.ValidatePassword(password, hash))
				return GetPlayerById(playerId.Value);

			return player;
		}

		public Player GetPlayerByToken(string token)
		{
			Player player = null;

			var key = $"token:{token}";
			var playerId = (int?)database.StringGet(key);

			if (playerId.HasValue)
				return GetPlayerById(playerId.Value);

			return player;
		}

		public List<Player> GetPlayersByPosition(Position position)
		{
			var playerIds = database.SetMembers($"position:{position.X}.{position.Y}.{position.Z}");

			return playerIds.Select(id => GetPlayerById(int.Parse(id))).Where(p => p != null).ToList();
		}

		public Player GetPlayerById(int playerId) 
		{
			Player player = null;
			var values = database.HashGet("player:" + playerId, playerFields);

			if (!values.FirstOrDefault().IsNull) 
			{
				player = new Player();
				player.Id = playerId;
				player.Name = values[0];
				player.Health = (int)values[1];
				player.Position = new Position() { X = (int)values[2], Y = (int)values[3], Z = (int)values[4] };
			}

			return player;
		}

		public List<Player> GetPlayers(Position position)
		{
			return new List<Player>();//players.Where(p => p.Value.Position.Equals(position)).Select(p => p.Value).ToList();
		}

		private int NextUserId() {
			var database = databaseService.GetDatabase();

			var id = (int?)database.StringIncrement("topid");

			if (!id.HasValue)
				throw new NullReferenceException("NextUserId topid returned a null id");

			return id.Value;
		}

	}
}