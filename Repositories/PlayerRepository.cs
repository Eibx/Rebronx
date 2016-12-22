using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;
using StackExchange.Redis;

namespace Rebronx.Server.Repositories
{
	public class PlayerRepository : IPlayerRepository
	{
		private readonly RedisValue[] playerFields;

		private readonly IDatabaseService databaseService;
		private readonly IDatabase database;
		private readonly ISocketRepository socketRepository;

		public PlayerRepository(IDatabaseService databaseService, ISocketRepository socketRepository)
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

		public void AddPlayer(Player player)
		{
			//NOTE only used for new players - right?
		}

		public Player GetPlayerByConnection(Guid connectionId)
		{
			var id = connectionId.ToString("N");
			var playerId = database.StringGet("connection:" + id);

			if (!playerId.IsNull && !playerId.IsInteger) {
				//TODO: Log error with playerid
				Console.WriteLine($"playerId was null or not and integer: {id}");
				return null;
			}

			return GetPlayerById(int.Parse(playerId));				
		}
		public Player GetPlayerByUsername(string username)
		{
			return new Player();
		}

		public List<Player> GetPlayersByPosition(Position position)
		{
			var playerIds = database.ListRange($"players:position:{position.X}.{position.Y}.{position.Y}");

			return playerIds.Select(id => GetPlayerById(int.Parse(id))).Where(p => p != null).ToList();
		}

		public Player GetPlayerById(int playerId) 
		{
			Player player = null;
			var values = database.HashGet("user:" + playerId, playerFields);

			if (!values.FirstOrDefault().IsNull) 
			{
				player = new Player();
				player.Id = playerId;
				player.Name = values[0];
				player.Health = int.Parse(values[1]);
				player.Position = new Position() { X = int.Parse(values[2]), Y = int.Parse(values[3]), Z = int.Parse(values[4]) };
			}

			return player;
		}

		public List<Player> GetPlayers(Position position)
		{
			return new List<Player>();//players.Where(p => p.Value.Position.Equals(position)).Select(p => p.Value).ToList();
		}

		public void RemovePlayer(int playerId) 
		{
			
		}

	}
}