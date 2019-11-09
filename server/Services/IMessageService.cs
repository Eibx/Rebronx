using Rebronx.Server.Models;

namespace Rebronx.Server.Services
{
    public interface IMessageService
    {
        void Send<T>(ClientConnection connection, string system, string type, T data);
        void Send<T>(Player player, string system, string type, T data);
        void SendPosition<T>(int node, string system, string type, T data);
        void SendAll<T>(string system, string type, T data);
    }
}