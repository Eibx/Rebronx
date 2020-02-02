using Rebronx.Server.Models;

namespace Rebronx.Server.Systems.Login.Senders
{
    public interface ILoginSender
    {
         void Success(Player player, string token);
         void Fail(ClientConnection connection, int reason);
         void Fail(Player player, int reason);

         void SignupSuccess(ClientConnection connection, string token);
         void SignupFail(ClientConnection connection, int reason);

         void Logout(ClientConnection connection);
    }
}