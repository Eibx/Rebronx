using System;
using System.Linq;
using System.Collections.Generic;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Repositories
{
	public class SocketRepository : ISocketRepository
    {
        private Dictionary<Guid, ClientConnection> sockets;
        private BiDictionary<int, Guid> playerSocketDictionary;

        public SocketRepository()
        {
            sockets = new Dictionary<Guid, ClientConnection>();
            playerSocketDictionary = new BiDictionary<int, Guid>();
        }

        public ClientConnection GetConnection(Guid connectionId)
        {
            ClientConnection connection = null;
            sockets.TryGetValue(connectionId, out connection);

            return connection;
        }

        public ClientConnection GetConnection(int playerId)
        {
            Guid connectionId = Guid.Empty;
            playerSocketDictionary.TryGetByFirst(playerId, out connectionId);

            return GetConnection(connectionId);
        }

        public bool IsPlayerOnline(int playerId)
        {
            return playerSocketDictionary.ContainsByFirst(playerId);
        }

        public int? GetPlayerId(Guid connectionId) {
            int playerId = 0;
            if (playerSocketDictionary.TryGetBySecond(connectionId, out playerId)) {
                return playerId;
            } else {
                return null;
            }
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
            Guid connectionId = Guid.Empty;

            if (playerSocketDictionary.TryGetByFirst(playerId, out connectionId)) {
                if (sockets.ContainsKey(connectionId)) {
                    sockets[connectionId].Client.Close();
                    sockets.Remove(connectionId);
                }
            }
        }
    }
}