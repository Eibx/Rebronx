using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Systems.Lobby.Senders
{
    public class LobbySender : ILobbySender
    {
        private readonly IPositionRepository positionRepository;
        private readonly IMessageService messageService;

        public LobbySender(IPositionRepository positionRepository, IMessageService messageService)
        {
            this.positionRepository = positionRepository;
            this.messageService = messageService;
        }

        public void Update(int node)
        {
            var players = positionRepository.GetPlayersByPosition(node);

            var sendLobbyMessage = new SendLobbyMessage() {
                Players = players.Select(p => new LobbyPlayer(p)).ToList()
            };

            messageService.SendPosition(node, "lobby", "lobby", sendLobbyMessage);
        }
    }

    public class SendLobbyMessage
    {
        public List<LobbyPlayer> Players { get; set; }
    }
}