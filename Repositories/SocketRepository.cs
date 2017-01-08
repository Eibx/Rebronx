using System;
using System.Linq;
using System.Collections.Generic;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using System.Net.Sockets;

namespace Rebronx.Server.Repositories
{
	public class SocketRepository : ISocketRepository
	{
		private Dictionary<Guid, SocketConnection> sockets;
		private BiDictionary<int, Guid> playerSocketDictionary;

		public SocketRepository()
		{
			sockets = new Dictionary<Guid, SocketConnection>();
			playerSocketDictionary = new BiDictionary<int, Guid>();
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

		public int? GetPlayerId(Guid connectionId) {
			int playerId = 0;
			if (playerSocketDictionary.TryGetBySecond(connectionId, out playerId)) {
				return playerId;
			} else {
				return null;	
			}
		}

		public List<SocketConnection> GetAllConnections()
		{
			return sockets.Select(x => x.Value).ToList();
		}

		public void AddConnection(int playerId, SocketConnection connection) 
		{
			if (playerSocketDictionary.ContainsByFirst(playerId)) 
			{
				playerSocketDictionary.RemoveByFirst(playerId);
			}
			
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
			if (sockets.ContainsKey(connectionId))
				sockets.Remove(connectionId);
				
			if (playerSocketDictionary.ContainsBySecond(connectionId))
				playerSocketDictionary.RemoveBySecond(connectionId);
		}

		public void RemoveConnection(int playerId) 
		{
			Guid connectionId = Guid.Empty;

			if (playerSocketDictionary.TryGetByFirst(playerId, out connectionId)) {
				if (sockets.ContainsKey(connectionId)) {
					sockets[connectionId].Socket.Shutdown(SocketShutdown.Both);
					sockets.Remove(connectionId);
				}
			}
		}
	}
}