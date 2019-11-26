using System;
using System.Collections.Generic;
using Rebronx.Server.Helpers;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Chat.Senders;

namespace Rebronx.Server.Systems.Chat.Senders
{
    public class ChatSender : IChatSender
    {
        private readonly IMessageService _messageService;

        private readonly List<Tuple<Player, string>> _messages = new List<Tuple<Player, string>>();

        private readonly TickGate _gate = new TickGate(100);
        public ChatSender(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void Say(Player player, string message)
        {
            _messages.Add(new Tuple<Player, string>(player, $"{player.Name}: {message}"));
        }

        public void Execute()
        {
            if (!_gate.Tick())
                return;

            foreach (var message in _messages)
            {
                var chatMessage = new ChatResponse();
                chatMessage.Message = message.Item2;
                _messageService.SendPosition(message.Item1.Node, "lobby", "chat", chatMessage);
            }

            _messages.Clear();
        }
    }

    public class ChatResponse
    {
        public string Message { get; set; }
    }
}