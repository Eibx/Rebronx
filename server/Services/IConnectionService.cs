using System.Collections.Generic;

namespace Rebronx.Server.Services
{
    public interface IConnectionService
    {
        void HandleDeadPlayers();
        List<Message> ConvertToMessages(List<WebSocketMessage> messages);
    }
}