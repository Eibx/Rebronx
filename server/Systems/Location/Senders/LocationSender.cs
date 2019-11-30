using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Helpers;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Location.Senders
{
    public class LocationSender : ILocationSender
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMessageService _messageService;

        private readonly HashSet<int> _nodesToUpdate = new HashSet<int>();

        private readonly TickGate _gate = new TickGate(100);

        public LocationSender(IPositionRepository positionRepository, IMessageService messageService)
        {
            _positionRepository = positionRepository;
            _messageService = messageService;
        }

        public void Execute()
        {
            if (!_gate.Tick())
                return;

            foreach (var node in _nodesToUpdate)
            {
                var players = _positionRepository.GetPlayersByPosition(node);

                var sendLocationMessage = new LocationResponse() {
                    Players = players.Select(p => new LobbyPlayer(p)).ToList()
                };

                _messageService.SendPosition(node, SystemTypes.Location, SystemTypes.LocationTypes.PlayersUpdate, sendLocationMessage);
            }

            _nodesToUpdate.Clear();
        }

        public void Update(int node)
        {
            _nodesToUpdate.Add(node);
        }
    }

    public class LocationResponse
    {
        public List<LobbyPlayer> Players { get; set; }
    }
}