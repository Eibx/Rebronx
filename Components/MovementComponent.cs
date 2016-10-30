using System;

public class MovementComponent : IMovementComponent
{
    private const string Component = "movement";
	private readonly IWebSocketCore webSocketCore;

	public MovementComponent(IWebSocketCore webSocketCore)
	{
		this.webSocketCore = webSocketCore; 
	}

    public void Run()
    {
		var messages = webSocketCore.GetMessages(Component);

		foreach (var message in messages)
		{
			if (message.Type == "move")
				MessageMove(message);
		}
    }

	public void MessageMove(WebSocketMessage message) 
	{
		
	}
}