using System.Collections.Generic;

namespace Rebronx.Server.Repositories.Interfaces
{
    public interface IPlayerRepository
    {
		void AddPlayer(string token, Player player);
		Player GetPlayer(string token);	
		Dictionary<string, Player> GetPlayers();
		List<Player> GetPlayers(Position position);
		void RemovePlayer(string token);
    }
}