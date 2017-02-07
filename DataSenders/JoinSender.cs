using System;
using System.Linq;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.DataSenders
{
	public class JoinSender : IJoinSender
	{
		private readonly IMessageService messageService;
		private readonly ILobbySender lobbySender;
		
		public JoinSender(IMessageService messageService, ILobbySender lobbySender)
		{
			this.messageService = messageService;
			this.lobbySender = lobbySender;
		}

		public void Join(Player player)
		{
			if (player != null) {
				var position = player.Position;
				var joinMessage = new SendJoinMessage();
				joinMessage.Id = player.Id;
				joinMessage.Name = player.Name;
				joinMessage.Position = position;

				//TODO: Send credits - CreditRepository?
				joinMessage.Credits = 0;

				messageService.Send(player, "join", "join", joinMessage);
				lobbySender.Update(position);
			}
			
		}
	}

	public class SendJoinMessage {
		public int Id { get; set; }
		public string Name { get; set; }
		public int Credits { get; set; }
		public Position Position { get; set; }
	}
}