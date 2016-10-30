using System;

public class ChatComponent : IChatComponent
{
	private const string Component = "chat";
	private readonly IWebSocketCore webSocketCore;

	public ChatComponent(IWebSocketCore webSocketCore)
	{
		this.webSocketCore = webSocketCore;
	}

    public void Run()
    {
		var messages = webSocketCore.GetMessages(Component);

		foreach (var message in messages)
		{
			if (message.Type == "say")
				MessageSay();
		}
    }

	public void MessageSay()
	{
		
	}
}