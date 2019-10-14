using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Services.Interfaces
{
    public interface IMessageService
    {
        void Send<T>(ClientConnection connection, string component, string type, T data);
        void Send<T>(Player player, string component, string type, T data);
        void SendPosition<T>(int node, string component, string type, T data);
        void SendAll<T>(string component, string type, T data);
    }
}