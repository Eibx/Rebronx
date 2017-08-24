using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Chat;
using Rebronx.Server.Components.Chat.Senders;

namespace Rebronx.Server.Components.Chat
{
	public class ChatComponent : Component, IChatComponent
	{
		private const string Component = "chat";
		private readonly IChatSender chatSender;

		public ChatComponent(IChatSender chatSender)
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
		public string Message { get; set; }
	}
}