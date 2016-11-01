using System.Collections.Generic;
using Rebronx.Server.Models;

public interface IWebSocketCore
{
	List<SocketConnection> GetNewConnections();
	List<WebSocketMessage> PollMessages();
}