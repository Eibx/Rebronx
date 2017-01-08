using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Repositories.Interfaces;

public class MovementComponent : Component, IMovementComponent
{
    private const string Component = "movement";
    private readonly IMovementSender movementSender;
	private readonly IMovementRepository movementRepository;

    public MovementComponent(IMovementSender movementSender, IMovementRepository movementRepository)
    {
        this.movementSender = movementSender;
		this.movementRepository = movementRepository;
    }

    public void Run(IList<Message> messages)
    {
        foreach (var message in messages.Where(m => m.Component == Component))
        {
            if (message.Type == "move")
                MessageMove(message);
        }
    }

    public void MessageMove(Message message)
    {
        var moveMessage = GetData<InputMoveMessage>(message);

        if (moveMessage != null)
        {
            var oldPosition = new Position()
            {
                X = message.Player.Position.X,
                Y = message.Player.Position.Y,
                Z = message.Player.Position.Z
            };
            var newPosition = new Position()
            {
                X = moveMessage.X,
                Y = moveMessage.Y,
                Z = moveMessage.Z
            };

			var distance = Math.Pow(Math.Abs(newPosition.X - oldPosition.X), 2) + Math.Pow(Math.Abs(newPosition.Y - oldPosition.Y), 2);
            if (distance == 1)
            {
				movementRepository.SetPlayerPositon(message.Player, newPosition);
                movementSender.Move(message.Player, oldPosition, newPosition);
            }
        }
    }
}

public class InputMoveMessage
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}