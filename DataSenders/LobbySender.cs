using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.DataSenders
{
	public class LobbySender : ILobbySender
	{
		private readonly IPlayerRepository playerRepository;
		private readonly IMessageService messageService;

		public LobbySender(IPlayerRepository playerRepository, IMessageService messageService)
		{
			this.playerRepository = playerRepository;
			this.messageService = messageService;
		}

		public void Update(Position position)
		{
			var players = playerRepository.GetPlayers(position);

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