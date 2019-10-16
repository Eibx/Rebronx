using System.Collections.Generic;

namespace Rebronx.Server.Services
{
    public interface IConnectionService
    {
        void HandleLoginMessage(WebSocketMessage loginMessage);
        void HandleDeadPlayers();
        List<Message> ConvertToMessages(List<WebSocketMessage> messages);
    }
}