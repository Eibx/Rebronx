using System;
using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories.Interfaces
{
	public interface ISocketRepository
	{
		SocketConnection GetConnection(Guid connectionId);
		SocketConnection GetConnection(int playerId);
		bool IsPlayerOnline(int playerId);
		int? GetPlayerId(Guid connectionId);
		List<SocketConnection> GetAllConnections();
		void AddConnection(int playerId, SocketConnection connection);
		void AddUnauthorizedConnection(SocketConnection connection);
		void RemoveConnection(Guid connectionId);
		void RemoveConnection(int playerId);
	}
}