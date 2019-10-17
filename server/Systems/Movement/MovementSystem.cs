using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Lobby;
using Rebronx.Server.Systems.Lobby.Senders;
using Rebronx.Server.Systems.Map.Services;
using Rebronx.Server.Systems.Movement.Senders;

namespace Rebronx.Server.Systems.Movement
{
    public class MovementSystem : System, IMovementSystem
    {
        private const string Component = "movement";
        private readonly IMovementSender _movementSender;
        private readonly ILobbySender _lobbySender;
        private readonly IPositionRepository _movementRepository;

        private readonly IMapService _mapService;

        private readonly Dictionary<int, MovementDistination> _movements;

        public MovementSystem(
            IMovementSender movementSender,
            ILobbySender lobbySender,
            IPositionRepository movementRepository,
            IMapService mapService)
        {
            _movementSender = movementSender;
            _lobbySender = lobbySender;
            _movementRepository = movementRepository;
            _mapService = mapService;

            _movements = new Dictionary<int, MovementDistination>();
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "move")
                    MessageMove(message);
            }

            foreach (var item in _movements.ToList())
            {
                if (item.Value.TravelTime <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    _movementRepository.SetPlayerPosition(item.Value.Player, item.Value.Node);
                    _movementSender.SetPosition(item.Value.Player, item.Value.Node);
                    _movements.Remove(item.Key);

                    _lobbySender.Update(item.Value.Player.Node);
                    _lobbySender.Update(item.Value.Node);
                }
            }
        }

        public void MessageMove(Message message)
        {
            var moveMessage = GetData<InputMoveMessage>(message);
            var player = message.Player;

            if (moveMessage == null)
                return;

            if (moveMessage.Nodes.Count < 2)
                return;

            var playerNode = message.Player.Node;

            if (playerNode != moveMessage.Nodes.First())
                return;

            float totalCost = 0.0f;
            for (int i = 1; i < moveMessage.Nodes.Count; i++)
            {
                var previousNode = _mapService.GetNode(moveMessage.Nodes[i-1]);
                var currentNode = _mapService.GetNode(moveMessage.Nodes[i]);

                if (previousNode == null || currentNode == null)
                    return;

                var connection = previousNode.Connections.FirstOrDefault(x => x.Id == currentNode.Id);
                if (connection == null)
                    return;

                totalCost += connection.Cost;
            }

            //TODO: Look at this.
            long travelTimeInMs = (long)totalCost * 1000;

            _movements[message.Player.Id] = new MovementDistination() {
                Node = moveMessage.Nodes.Last(),
                TravelTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + travelTimeInMs,
                Player = player
            };

            // Start move and actual move
            _movementSender.StartMove(message.Player, moveMessage.Nodes, travelTimeInMs);
        }
    }

    public class InputMoveMessage
    {
        public List<int> Nodes { get; set; }
    }

    public class MovementDistination
    {
        public int Node { get; set; }
        public long TravelTime { get; set; }
        public Player Player { get; set; }
    }
}