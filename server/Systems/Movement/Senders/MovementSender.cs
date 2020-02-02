using System.Collections.Generic;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;
using NotImplementedException = System.NotImplementedException;

namespace Rebronx.Server.Systems.Movement.Senders
{
    public class MovementSender : IMovementSender
    {
        private readonly IMessageService _messageService;
        private readonly IMovementRepository _movementRepository;

        public MovementSender(
            IMessageService messageService,
            IMovementRepository movementRepository)
        {
            _messageService = messageService;
            _movementRepository = movementRepository;
        }

        public void StartMove(Player player)
        {
            var movement = _movementRepository.Get(player.Id);

            if (movement == null)
                return;

            var movementMessage = new StartMoveResponse()
            {
                Nodes = movement.Nodes,
                StartTime = movement.StartTime,
                MoveTime = movement.TravelTime,
            };

            _messageService.Send(player, SystemTypes.Movement, SystemTypes.MovementTypes.StartMove, movementMessage);
        }

        public void SetPosition(Player player, int newNode)
        {
            var movementMessage = new SendPositionResponse()
            {
                Node = newNode
            };

            _messageService.Send(player, SystemTypes.Movement, SystemTypes.MovementTypes.MoveDone, movementMessage);
        }
    }

    public class StartMoveResponse
    {
        public long StartTime { get; set; }
        public long MoveTime { get; set; }
        public List<int> Nodes { get; set; }
    }

    public class SendPositionResponse
    {
        public int Node { get; set; }
    }
}