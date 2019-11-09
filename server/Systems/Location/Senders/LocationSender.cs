using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Location.Senders
{
    public class LocationSender : ILocationSender
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMessageService _messageService;

        public LocationSender(IPositionRepository positionRepository, IMessageService messageService)
        {
            _positionRepository = positionRepository;
            _messageService = messageService;
        }

        public void Update(int node)
        {
            var players = _positionRepository.GetPlayersByPosition(node);

            var sendLocationMessage = new LocationResponse() {
                Players = players.Select(p => new LobbyPlayer(p)).ToList()
            };

            _messageService.SendPosition(node, SystemNames.Location, "location", sendLocationMessage);
        }
    }

    public class LocationResponse
    {
        public List<LobbyPlayer> Players { get; set; }
    }
}