using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Location.Senders;
using Rebronx.Server.Systems.Map.Services;
using Rebronx.Server.Systems.Movement.Senders;

namespace Rebronx.Server.Systems.Movement
{
    public class MovementSystem : System, IMovementSystem
    {
        private readonly IMovementSender _movementSender;
        private readonly ILocationSender _locationSender;
        private readonly IPositionRepository _movementRepository;

        private readonly IMapService _mapService;

        private readonly Dictionary<int, MovementDestination> _movements;

        public MovementSystem(
            IMovementSender movementSender,
            ILocationSender locationSender,
            IPositionRepository movementRepository,
            IMapService mapService)
        {
            _movementSender = movementSender;
            _locationSender = locationSender;
            _movementRepository = movementRepository;
            _mapService = mapService;

            _movements = new Dictionary<int, MovementDestination>();
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemNames.Movement))
            {
                switch (message.Type)
                {
                    case "move":
                        ProcessMovementRequest(message);
                        break;
                }
            }

            foreach (var (key, movement) in _movements.ToList())
            {
                if (movement.TravelTime <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    _movementRepository.SetPlayerPosition(movement.Player, movement.Node);

                    _movementSender.SetPosition(movement.Player, movement.Node);
                    _movements.Remove(key);

                    _locationSender.Update(movement.Player.Node);
                    _locationSender.Update(movement.Node);
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

            float totalCost = 0.0f;
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

            long travelTimeInMs = (long)totalCost * 1000;

            _movements[message.Player.Id] = new MovementDestination() {
                Node = nodes.Last(),
                TravelTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + travelTimeInMs,
                Player = player
            };

            // Start move and actual move
            _movementSender.StartMove(message.Player, nodes, travelTimeInMs);
        }
    }

    public class MoveRequest
    {
        public List<int> Nodes { get; set; }
    }

    public class MovementDestination
    {
        public int Node { get; set; }
        public long TravelTime { get; set; }
        public Player Player { get; set; }
    }
}