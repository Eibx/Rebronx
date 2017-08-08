using System;
using System.Collections.Generic;
using Rebronx.Server.Components.Lobby;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Movement
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

		public void StartMove(Player player, int newPosition, long moveTime)
		{
			var movementMessage = new SendStartMoveMessage() {
				Position = newPosition,
				MoveTime = moveTime
			};

			messageService.Send(player, "player", "movement", movementMessage);
		}

		public void SetPosition(Player player, int newPosition) {
			var movementMessage = new SendPositionMessage() {
				Position = newPosition
			};

			messageService.Send(player, "player", "position", movementMessage);

		}
	}

	public class SendStartMoveMessage
	{
		public int Position { get; set; }
		public long MoveTime { get; set; }
	}
	
	public class SendPositionMessage
	{
		public int Position { get; set; }
	}
}