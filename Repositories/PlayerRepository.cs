using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
		private Dictionary<string, Player> players;

		public PlayerRepository()
		{
			players = new Dictionary<string, Player>();	
		}

		public void AddPlayer(string token, Player player)
		{
			if (players.ContainsKey(token))
			{
				players[token] = player;
			}
			else
			{
				players.Add(token, player);
			}
		}

		public Player GetPlayer(string token)
		{
			Player player = null;
			players.TryGetValue(token, out player);
				
			return player;
		}

		public List<Player> GetPlayers(Position position)
		{
			return players.Where(p => p.Value.Position.Equals(position)).Select(p => p.Value).ToList();
		}

		public Dictionary<string, Player> GetPlayers() 
		{
			return players;
		}

		public void RemovePlayer(string token) 
		{
			if (players.ContainsKey(token))
			{
				players.Remove(token);
			}
		}
    }
}