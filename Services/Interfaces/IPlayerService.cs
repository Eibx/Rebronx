using System.Collections.Generic;
using Rebronx.Server.Models;

public interface IPlayerService
{
	void HandleNewPlayers(List<SocketConnection> connections);

	List<Message> ConvertToMessages(List<WebSocketMessage> messages);
}