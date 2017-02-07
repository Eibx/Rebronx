using System;
using System.Collections.Generic;

namespace Rebronx.Server.Repositories.Interfaces
{
	public interface IUserRepository
	{
		void CreateNewPlayer(string username, string hash, string token);
		Player GetPlayerById(int playerId);
		Player GetPlayerByUsername(string username);
		Player GetPlayerByLogin(string username, string password);
		Player GetPlayerByToken(string token);
		List<Player> GetPlayersByPosition(Position position);
		List<Player> GetPlayers(Position position);
		void RemovePlayer(int playerId);
	}
}