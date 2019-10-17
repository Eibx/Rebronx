using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Lobby.Senders
{
    public class LobbySender : ILobbySender
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMessageService _messageService;

        public LobbySender(IPositionRepository positionRepository, IMessageService messageService)
        {
            _positionRepository = positionRepository;
            _messageService = messageService;
        }

        public void Update(int node)
        {
            var players = _positionRepository.GetPlayersByPosition(node);

            var sendLobbyMessage = new SendLobbyMessage() {
                Players = players.Select(p => new LobbyPlayer(p)).ToList()
            };

            _messageService.SendPosition(node, "lobby", "lobby", sendLobbyMessage);
        }
    }

    public class SendLobbyMessage
    {
        public List<LobbyPlayer> Players { get; set; }
    }
}