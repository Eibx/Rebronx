using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Systems.Chat;
using Rebronx.Server.Systems.Chat.Senders;

namespace Rebronx.Server.Systems.Chat
{
    public class ChatSystem : System, IChatSystem
    {
        private const string Component = "chat";
        private readonly IChatSender chatSender;

        public ChatSystem(IChatSender chatSender)
        {
            this.chatSender = chatSender;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "say")
                    MessageSay(message);
            }
        }

        public void MessageSay(Message message)
        {
            var inputMessage = GetData<InputChatMessage>(message);

            if (inputMessage != null && message?.Player != null)
            {
                Console.WriteLine(message.Player.Name + ": " + inputMessage.Message);
                chatSender.Say(message.Player, inputMessage.Message);
            }
        }
    }
    public class InputChatMessage
    {
        public string Message { get; set; } = "";
    }
}