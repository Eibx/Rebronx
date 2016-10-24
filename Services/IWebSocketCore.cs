using System.Collections.Generic;

public interface IWebSocketCore
{
	void HandleNewConnections();
	List<WebSocketMessage> GetMessages(string component);
	void PollMessages();
}