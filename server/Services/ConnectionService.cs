using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Lobby.Senders;

namespace Rebronx.Server.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IUserRepository _playerRepository;
        private readonly ISocketRepository _socketRepository;
        private readonly ILobbySender _lobbySender;

        public ConnectionService(
            IUserRepository playerRepository,
            ISocketRepository socketRepository,
            ILobbySender lobbySender)
        {
            _socketRepository = socketRepository;
            _playerRepository = playerRepository;
            _lobbySender = lobbySender;
        }

        public List<Message> ConvertToMessages(List<WebSocketMessage> messages)
        {
            var output = new List<Message>();

            foreach (var message in messages)
            {
                if (message.Type == "login" || message.Type == "signup")
                {
                    output.Add(new UnauthorizedMessage
                    {
                        Player = null,
                        Connection = message.Connection,
                        Component = message.Component,
                        Type = message.Type,
                        Data = message.Data
                    });

                    continue;
                }

                Player player = null;

                var playerId = _socketRepository.GetPlayerId(message.Connection.Id);

                if (playerId != null)
                    player = _playerRepository.GetPlayerById(playerId.Value);

                if (player == null)
                    continue;

                output.Add(new Message
                {
                    Player = player,
                    Component = message.Component,
                    Type = message.Type,
                    Data = message.Data
                });
            }

            return output;
        }

        public void HandleDeadPlayers()
        {
            var connections = _socketRepository.GetAllConnections();
            var timeouts = connections.Where(x => x.TcpClient == null || x.IsTimedOut()).ToList();

            foreach (var timeout in timeouts)
            {
                Player player = null;
                var playerId = _socketRepository.GetPlayerId(timeout.Id);

                if (playerId.HasValue)
                {
                    player = _playerRepository.GetPlayerById(playerId.Value);
                }

                Console.WriteLine($"Connection removed: {timeout.Id} - {timeout.LastMessage}");
                _socketRepository.RemoveConnection(timeout.Id);

                if (player != null)
                {
                    _lobbySender.Update(player.Node);
                }
            }
        }
    }
}