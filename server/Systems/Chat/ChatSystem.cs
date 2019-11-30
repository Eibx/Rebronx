using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Systems.Chat;
using Rebronx.Server.Systems.Chat.Senders;

namespace Rebronx.Server.Systems.Chat
{
    public class ChatSystem : System, IChatSystem
    {
        private readonly IChatSender _chatSender;

        public ChatSystem(IChatSender chatSender)
        {
            _chatSender = chatSender;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemTypes.Chat))
            {
                if (message.Type == SystemTypes.ChatTypes.Say)
                    MessageSay(message);
            }
        }

        private void MessageSay(Message message)
        {
            var inputMessage = GetData<ChatRequest>(message);

            if (inputMessage != null && message?.Player != null)
            {
                Console.WriteLine(message.Player.Name + ": " + inputMessage.Message);
                _chatSender.Say(message.Player, inputMessage.Message);
            }
        }
    }
    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}