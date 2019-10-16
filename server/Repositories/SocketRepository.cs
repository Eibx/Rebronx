using System;
using System.Linq;
using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories
{
	public class SocketRepository : ISocketRepository
    {
        private readonly Dictionary<Guid, ClientConnection> sockets;
        private readonly BiDictionary<int, Guid> playerSocketDictionary;

        public SocketRepository()
        {
            sockets = new Dictionary<Guid, ClientConnection>();
            playerSocketDictionary = new BiDictionary<int, Guid>();
        }

        public ClientConnection GetConnection(Guid connectionId)
        {
            sockets.TryGetValue(connectionId, out var connection);

            return connection;
        }

        public ClientConnection GetConnection(int playerId)
        {
            playerSocketDictionary.TryGetByFirst(playerId, out var connectionId);

            return GetConnection(connectionId);
        }

        public bool IsPlayerOnline(int playerId)
        {
            return playerSocketDictionary.ContainsByFirst(playerId);
        }

        public int? GetPlayerId(Guid connectionId)
        {
            if (playerSocketDictionary.TryGetBySecond(connectionId, out var playerId))
            {
                return playerId;
            }

            return null;
        }

        public List<ClientConnection> GetAllConnections()
        {
            return sockets.Select(x => x.Value).ToList();
        }

        public void AddConnection(int playerId, ClientConnection connection)
        {
            if (playerSocketDictionary.ContainsByFirst(playerId))
            {
                playerSocketDictionary.RemoveByFirst(playerId);
            }

            playerSocketDictionary.Add(playerId, connection.Id);

            if (!sockets.ContainsKey(connection.Id))
                sockets.Add(connection.Id, connection);
        }

        public void AddUnauthorizedConnection(ClientConnection connection)
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
            if (playerSocketDictionary.TryGetByFirst(playerId, out var connectionId))
            {
                if (sockets.ContainsKey(connectionId))
                {
                    sockets[connectionId].Client.Close();
                    sockets.Remove(connectionId);
                }
            }
        }
    }
}