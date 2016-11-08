using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Services.Interfaces
{
    public interface IConnectionService
    {
        void HandleNewPlayers(List<SocketConnection> connections);
		void HandleDeadPlayers();
        List<Message> ConvertToMessages(List<WebSocketMessage> messages);
    }
}