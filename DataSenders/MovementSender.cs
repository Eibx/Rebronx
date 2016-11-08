using System;
using System.Collections.Generic;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.DataSenders
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

		public void Move(Player player, Position fromPosition, Position toPosition)
		{
			var positionMessage = new SendPositionMessage() {
				X = toPosition.X,
				Y = toPosition.Y,
				Z = toPosition.Z,
			};

			lobbySender.Update(fromPosition);
			lobbySender.Update(toPosition);
			
			messageService.Send(player, "player", "position", positionMessage);
		}
	}

	public class SendPositionMessage
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
	}
}