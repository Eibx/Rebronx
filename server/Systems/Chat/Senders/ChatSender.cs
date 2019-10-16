using System;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Chat.Senders;

namespace Rebronx.Server.Systems.Chat.Senders
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

            messageService.SendPosition(player.Node, "lobby", "chat", chatMessage);
        }
    }

    public class SendChatMessage
    {
        public string Message { get; set; }
    }
}