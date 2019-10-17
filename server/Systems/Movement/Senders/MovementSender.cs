using System.Collections.Generic;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Movement.Senders
{
    public class MovementSender : IMovementSender
    {
        private readonly IMessageService _messageService;

        public MovementSender(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void StartMove(Player player, List<int> nodes, long moveTime)
        {
            var movementMessage = new SendStartMoveMessage()
            {
                Nodes = nodes,
                MoveTime = moveTime
            };

            _messageService.Send(player, "player", "movement", movementMessage);
        }

        public void SetPosition(Player player, int newNode)
        {
            var movementMessage = new SendPositionMessage()
            {
                Node = newNode
            };

            _messageService.Send(player, "player", "position", movementMessage);

        }
    }

    public class SendStartMoveMessage
    {
        public long MoveTime { get; set; }
        public List<int> Nodes { get; set; }
    }

    public class SendPositionMessage
    {
        public int Node { get; set; }
    }
}