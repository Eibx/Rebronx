using System;
using System.Collections.Generic;
using System.Linq;

public class MovementComponent : IMovementComponent
{
    private const string Component = "movement";
	private readonly IWebSocketCore webSocketCore;

	public MovementComponent(IWebSocketCore webSocketCore)
	{
		this.webSocketCore = webSocketCore; 
	}

    public void Run(IList<Message> messages)
    {
		foreach (var message in messages.Where(m => m.Component == Component))
		{
			if (message.Type == "move")
				MessageMove(message);
		}
    }

	public void MessageMove(Message message) 
	{
		Console.WriteLine(message.Data);
	}
}