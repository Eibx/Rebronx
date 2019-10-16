using System.Collections.Generic;
using System.Net.Security;

namespace Rebronx.Server.Services
{
    public interface IWebSocketCore
    {
        void GetNewConnections();
        List<WebSocketMessage> PollMessages();
        void Send(SslStream client, string data);
    }
}