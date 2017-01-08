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
			var position = player?.Position;
			if (position != null)
			{
				var joinMessage = new SendJoinMessage();
				joinMessage.Position = position;

				Console.WriteLine($"JoinSender: {position.X} {position.Y} {position.Z}");
				messageService.Send(player, "join", "join", joinMessage);
				lobbySender.Update(position);
			}
		}
	}

	public class SendJoinMessage {
		public Position Position { get; set; }
	}
}