using System;
using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories.Interfaces
{
	public interface ISocketRepository
	{
		ClientConnection GetConnection(Guid connectionId);
		ClientConnection GetConnection(int playerId);
		bool IsPlayerOnline(int playerId);
		int? GetPlayerId(Guid connectionId);
		List<ClientConnection> GetAllConnections();
		void AddConnection(int playerId, ClientConnection connection);
		void AddUnauthorizedConnection(ClientConnection connection);
		void RemoveConnection(Guid connectionId);
		void RemoveConnection(int playerId);
	}
}