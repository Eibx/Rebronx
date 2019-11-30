using Rebronx.Server.Enums;
using Rebronx.Server.Models;

namespace Rebronx.Server.Services
{
    public interface IMessageService
    {
        void Send<T>(ClientConnection connection, byte system, byte type, T data);
        void Send<T>(Player player, byte system, byte type, T data);
        void SendPosition<T>(int node, byte system, byte type, T data);
        void SendAll<T>(byte system, byte type, T data);
    }
}