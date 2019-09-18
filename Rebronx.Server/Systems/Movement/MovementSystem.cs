using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Systems.Lobby;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;
using Rebronx.Server.Systems.Lobby.Senders;
using Rebronx.Server.Systems.Map.Services;
using Rebronx.Server.Systems.Movement.Senders;

namespace Rebronx.Server.Systems.Movement
{
    public class MovementSystem : System, IMovementSystem
    {
        private const string Component = "movement";
        private readonly IMovementSender movementSender;
        private readonly ILobbySender lobbySender;
        private readonly IPositionRepository movementRepository;

        private readonly IMapService mapService;

        private readonly Dictionary<int, MovementDistination> movements;

        public MovementSystem(
            IMovementSender movementSender,
            ILobbySender lobbySender,
            IPositionRepository movementRepository,
            IMapService mapService)
        {
            this.movementSender = movementSender;
            this.lobbySender = lobbySender;
            this.movementRepository = movementRepository;
            this.mapService = mapService;

            this.movements = new Dictionary<int, MovementDistination>();
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "move")
                    MessageMove(message);
            }

            foreach (var item in movements.ToList())
            {
                if (item.Value.TravelTime <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                {
                    movementRepository.SetPlayerPositon(item.Value.Player, item.Value.Position);
                    movementSender.SetPosition(item.Value.Player, item.Value.Position);
                    movements.Remove(item.Key);

                    lobbySender.Update(item.Value.Player.Position);
                    lobbySender.Update(item.Value.Position);
                }
            }
        }

        public void MessageMove(Message message)
        {
            var moveMessage = GetData<InputMoveMessage>(message);
            var player = message.Player;

            if (moveMessage == null)
                return;

            var currentNode = mapService.GetNode(message.Player.Position.X);
            var nextNode = mapService.GetNode(moveMessage.Position.X);

            if (currentNode == null || nextNode == null)
                return;

            if (!currentNode.Connections.Contains(nextNode.Id))
                return;

            //TODO: make cleaner
            var distanceX = Math.Pow(Math.Abs(nextNode.X-currentNode.X), 2);
            var distanceY = Math.Pow(Math.Abs(nextNode.Y-currentNode.Y), 2);
            var distance = Math.Sqrt(distanceX + distanceY);

            long travelTime = (long)Math.Round(distance * 5d);

            movements[message.Player.Id] = new MovementDistination() {
                Position = moveMessage.Position,
                TravelTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + travelTime,
                Player = player
            };

            // Start move and actual move
            movementSender.StartMove(message.Player, moveMessage.Position, travelTime);
        }
    }

    public class InputMoveMessage
    {
        public Position Position { get; set; }
    }

    public class MovementDistination
    {
        public Position Position { get; set; }
        public long TravelTime { get; set; }
        public Player Player { get; set; }
    }
}