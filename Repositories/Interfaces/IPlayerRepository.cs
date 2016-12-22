using System;
using System.Collections.Generic;

namespace Rebronx.Server.Repositories.Interfaces
{
	public interface IPlayerRepository
	{
		void AddPlayer(Player player);
		Player GetPlayerById(int playerId);	
		Player GetPlayerByConnection(Guid connectionId);
		Player GetPlayerByUsername(string username);
		List<Player> GetPlayersByPosition(Position position);
		List<Player> GetPlayers(Position position);
		void RemovePlayer(int playerId);
	}
}