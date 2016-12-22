using System.Collections.Generic;
using System.Net.Sockets;
using Rebronx.Server.Models;

public interface IWebSocketCore
{
	void GetNewConnections();
	List<WebSocketMessage> PollMessages();
	void Send(Socket socket, string data);
}