using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Components.Movement
{
	public class MovementComponent : Component, IMovementComponent
	{
		private const string Component = "movement";
		private readonly IMovementSender movementSender;
		private readonly IPositionRepository movementRepository;
		private readonly ICooldownRepository cooldownRepository;

		private readonly Dictionary<int, MovementDistination> movements; 

		public MovementComponent(IMovementSender movementSender, IPositionRepository movementRepository, ICooldownRepository cooldownRepository)
		{
			this.movementSender = movementSender;
			this.movementRepository = movementRepository;
			this.cooldownRepository = cooldownRepository;

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
				}
			}
		}

		public void MessageMove(Message message)
		{
			var moveMessage = GetData<InputMoveMessage>(message);
			var player = message.Player;

			if (moveMessage != null)
			{
				//TODO: check if there's a link between the two nodes.
				long travelTime = 1500; // calculate distance between points

				movements[message.Player.Id] = new MovementDistination() {
					Position = moveMessage.Position,
					TravelTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + travelTime,
					Player = player
				};

				// Start move and actual move
				movementSender.StartMove(message.Player, moveMessage.Position, travelTime);
			}
		}
	}

	public class InputMoveMessage
	{
		public int Position { get; set; }
	}

	public class MovementDistination
	{
		public int Position { get; set; }
		public long TravelTime { get; set; }
		public Player Player { get; set; }
	}
}