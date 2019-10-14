using Rebronx.Server.Services.Interfaces;
using Rebronx.Server.Systems.Lobby.Senders;

namespace Rebronx.Server.Systems.Movement.Senders
{
    public class MovementSender : IMovementSender
    {
        private readonly IMessageService messageService;
        private readonly ILobbySender lobbySender;

        public MovementSender(IMessageService messageService, ILobbySender lobbySender)
        {
            this.messageService = messageService;
            this.lobbySender = lobbySender;
        }

        public void StartMove(Player player, long moveTime)
        {
            var movementMessage = new SendStartMoveMessage()
            {
                MoveTime = moveTime
            };

            messageService.Send(player, "player", "movement", movementMessage);
        }

        public void SetPosition(Player player, int newNode)
        {
            var movementMessage = new SendPositionMessage()
            {
                Node = newNode
            };

            messageService.Send(player, "player", "position", movementMessage);

        }
    }

    public class SendStartMoveMessage
    {
        public long MoveTime { get; set; }
    }

    public class SendPositionMessage
    {
        public int Node { get; set; }
    }
}