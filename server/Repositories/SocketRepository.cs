using System;
using System.Linq;
using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories
{
	public class SocketRepository : ISocketRepository
    {
        private readonly Dictionary<Guid, ClientConnection> _sockets;
        private readonly BiDictionary<int, Guid> _playerSocketDictionary;

        public SocketRepository()
        {
            _sockets = new Dictionary<Guid, ClientConnection>();
            _playerSocketDictionary = new BiDictionary<int, Guid>();
        }

        public ClientConnection GetConnection(Guid connectionId)
        {
            _sockets.TryGetValue(connectionId, out var connection);

            return connection;
        }

        public ClientConnection GetConnection(int playerId)
        {
            _playerSocketDictionary.TryGetByFirst(playerId, out var connectionId);

            return GetConnection(connectionId);
        }

        public bool IsPlayerOnline(int playerId)
        {
            return _playerSocketDictionary.ContainsByFirst(playerId);
        }

        public int? GetPlayerId(Guid connectionId)
        {
            if (_playerSocketDictionary.TryGetBySecond(connectionId, out var playerId))
            {
                return playerId;
            }

            return null;
        }

        public List<ClientConnection> GetAllConnections()
        {
            return _sockets.Select(x => x.Value).ToList();
        }

        public void AddConnection(int playerId, ClientConnection connection)
        {
            if (_playerSocketDictionary.ContainsByFirst(playerId))
            {
                _playerSocketDictionary.RemoveByFirst(playerId);
            }

            _playerSocketDictionary.Add(playerId, connection.Id);

            if (!_sockets.ContainsKey(connection.Id))
                _sockets.Add(connection.Id, connection);
        }

        public void AddUnauthorizedConnection(ClientConnection connection)
        {
            _sockets.Add(connection.Id, connection);
        }

        public void RemoveConnection(Guid connectionId)
        {
            if (_sockets.ContainsKey(connectionId))
                _sockets.Remove(connectionId);

            if (_playerSocketDictionary.ContainsBySecond(connectionId))
                _playerSocketDictionary.RemoveBySecond(connectionId);
        }

        public void RemoveConnection(int playerId)
        {
            if (_playerSocketDictionary.TryGetByFirst(playerId, out var connectionId))
            {
                if (_sockets.ContainsKey(connectionId))
                {
                    _sockets[connectionId].Client.Close();
                    _sockets.Remove(connectionId);
                }
            }
        }
    }
}