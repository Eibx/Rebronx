using System;
using Rebronx.Server.Components.Chat.Senders;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Chat.Senders
{
	public class ChatSender : IChatSender
	{
		private readonly IMessageService messageService;
		public ChatSender(IMessageService messageService)
		{
			this.messageService = messageService;
		}

		public void Say(Player player, string message)
		{
			var chatMessage = new SendChatMessage();
			chatMessage.Message = $"{player.Name}: {message}";

			var position = player?.Position;
			if (position != null)
				messageService.SendPosition(player.Position, "lobby", "chat", chatMessage);
		}
	}

	public class SendChatMessage
	{
		public string Message { get; set; }
	}
}