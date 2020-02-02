using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Location.Senders;
using Rebronx.Server.Systems.Movement.Senders;

namespace Rebronx.Server.Systems.Movement
{
    public class MovementSystem : System, IMovementSystem
    {
        private readonly IMovementSender _movementSender;
        private readonly ILocationSender _locationSender;
        private readonly IMovementRepository _movementRepository;
        private readonly IPositionRepository _positionRepository;

        private readonly IMapService _mapService;

        public MovementSystem(
            IMovementSender movementSender,
            ILocationSender locationSender,
            IMovementRepository movementRepository,
            IMapService mapService, IPositionRepository positionRepository)
        {
            _movementSender = movementSender;
            _locationSender = locationSender;
            _movementRepository = movementRepository;
            _mapService = mapService;
            _positionRepository = positionRepository;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemTypes.Movement))
            {
                switch (message.Type)
                {
                    case SystemTypes.MovementTypes.Move:
                        ProcessMovementRequest(message);
                        break;
                }
            }

            foreach (var (key, movement) in _movementRepository.GetAll())
            {
                var traveledTime = (movement.StartTime + movement.TravelTime);
                if (traveledTime <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    var node = movement.Nodes.Last();
                    _positionRepository.SetPlayerPosition(movement.Player, node);

                    _movementRepository.Remove(key);

                    _movementSender.SetPosition(movement.Player, node);

                    _locationSender.Update(movement.Player.Node);
                    _locationSender.Update(node);
                }
            }
        }

        private void ProcessMovementRequest(Message message)
        {
            var moveMessage = GetData<MoveRequest>(message);
            var player = message.Player;

            if (moveMessage == null)
                return;

            var nodes = moveMessage.Nodes;
            if (nodes.Count < 2 || nodes.Count > 50)
                return;

            var playerNode = message.Player.Node;

            if (playerNode != nodes.First())
                return;

            var totalCost = 0.0f;
            for (var i = 1; i < nodes.Count; i++)
            {
                var previousNode = _mapService.GetNode(nodes[i-1]);
                var currentNode = _mapService.GetNode(nodes[i]);

                if (previousNode == null || currentNode == null)
                    return;

                var connection = previousNode.Connections.FirstOrDefault(x => x.Id == currentNode.Id);
                if (connection == null)
                    return;

                totalCost += connection.Cost;
            }

            var travelTimeInMs = (long)totalCost * 1000;

            var movement = new MovementDestination() {
                Nodes = nodes,
                StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                TravelTime = travelTimeInMs,
                Player = player
            };

            _movementRepository.Add(player.Id, movement);

            _movementSender.StartMove(player);
        }
    }

    public class MoveRequest
    {
        public List<int> Nodes { get; set; }
    }

    public class MovementDestination
    {
        public List<int> Nodes { get; set; }
        public long StartTime { get; set; }
        public long TravelTime { get; set; }
        public Player Player { get; set; }
    }
}