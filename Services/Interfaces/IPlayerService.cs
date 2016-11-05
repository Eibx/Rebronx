using System.Collections.Generic;
using Rebronx.Server.Models;

public interface IPlayerService
{
	void HandleNewPlayers(List<SocketConnection> connections);
	List<Message> ConvertToMessages(List<WebSocketMessage> messages);
	void Send(Player player, string component, string type, string data);
	void SendPosition<T>(Position position, string component, string type, T data);
	void SendAll<T>(string component, string type, T data);
}