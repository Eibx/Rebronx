using System;
using System.Collections.Generic;
using System.Linq;

public class ChatComponent : IChatComponent
{
	private const string Component = "chat";
	private readonly IWebSocketCore webSocketCore;

	public ChatComponent(IWebSocketCore webSocketCore)
	{
		this.webSocketCore = webSocketCore;
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
		Console.WriteLine(message.Data);
	}
}