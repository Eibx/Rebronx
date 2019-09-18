using Rebronx.Server.Components.Lobby.Senders;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Movement.Senders
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

        public void StartMove(Player player, Position newPosition, long moveTime)
        {
            var movementMessage = new SendStartMoveMessage()
            {
                Position = newPosition,
                MoveTime = moveTime
            };

            messageService.Send(player, "player", "movement", movementMessage);
        }

        public void SetPosition(Player player, Position newPosition)
        {
            var movementMessage = new SendPositionMessage()
            {
                Position = newPosition
            };

            messageService.Send(player, "player", "position", movementMessage);

        }
    }

    public class SendStartMoveMessage
    {
        public Position Position { get; set; }
        public long MoveTime { get; set; }
    }

    public class SendPositionMessage
    {
        public Position Position { get; set; }
    }
}