using System;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Chat.Senders;

namespace Rebronx.Server.Systems.Chat.Senders
{
    public class ChatSender : IChatSender
    {
        private readonly IMessageService _messageService;
        public ChatSender(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void Say(Player player, string message)
        {
            var chatMessage = new ChatResponse();
            chatMessage.Message = $"{player.Name}: {message}";

            _messageService.SendPosition(player.Node, "lobby", "chat", chatMessage);
        }
    }

    public class ChatResponse
    {
        public string Message { get; set; }
    }
}