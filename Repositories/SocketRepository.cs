using System;
using System.Linq;
using System.Collections.Generic;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Repositories
{
	public class SocketRepository : ISocketRepository
	{
		private Dictionary<Guid, SocketConnection> sockets;
		private BiDictionary<int, Guid> playerSocketDictionary;

		public SocketRepository()
		{
			sockets = new Dictionary<Guid, SocketConnection>();
		}

		public SocketConnection GetConnection(Guid connectionId) 
		{
			SocketConnection connection = null;
			sockets.TryGetValue(connectionId, out connection);

			return connection;
		}

		public SocketConnection GetConnection(int playerId) 
		{
			Guid connectionId = Guid.Empty;
			playerSocketDictionary.TryGetByFirst(playerId, out connectionId);

			return GetConnection(connectionId);
		}

		public List<SocketConnection> GetAllConnections()
		{
			return sockets.Select(x => x.Value).ToList();
		}

		public void AddConnection(int playerId, SocketConnection connection) 
		{
			playerSocketDictionary.Add(playerId, connection.Id);

			if (!sockets.ContainsKey(connection.Id))
				sockets.Add(connection.Id, connection);
		}

		public void AddUnauthorizedConnection(SocketConnection connection) 
		{
			sockets.Add(connection.Id, connection);
		}

		public void RemoveConnection(Guid connectionId) 
		{
			sockets.Remove(connectionId);
			playerSocketDictionary.RemoveBySecond(connectionId);
		}

		public void RemoveConnection(int playerId) 
		{
			Guid connectionId = Guid.Empty;
			
			if (playerSocketDictionary.TryGetByFirst(playerId, out connectionId))
				sockets.Remove(connectionId);
		}
	}
}