using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Lobby
{
	public class LobbySender : ILobbySender
	{
		private readonly IPositionRepository positionRepository;
		private readonly IMessageService messageService;

		public LobbySender(IPositionRepository positionRepository, IMessageService messageService)
		{
			this.positionRepository = positionRepository;
			this.messageService = messageService;
		}

		public void Update(int position)
		{
			var players = positionRepository.GetPlayersByPosition(position);

			var sendLobbyMessage = new SendLobbyMessage() {
				Players = players.Select(p => new LobbyPlayer(p)).ToList()
			};
			
			messageService.SendPosition(position, "lobby", "lobby", sendLobbyMessage);
		}
	}

	public class SendLobbyMessage 
	{
		public List<LobbyPlayer> Players { get; set; }
	}
}