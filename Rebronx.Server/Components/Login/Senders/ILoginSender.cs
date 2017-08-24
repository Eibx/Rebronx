using Rebronx.Server.Models;

namespace Rebronx.Server.Components.Login.Senders
{
	public interface ILoginSender
	{
		 void Success(Player player, string token);
		 void Fail(SocketConnection connection, int reason);
		 void Fail(Player player, int reason);

		 void SignupSuccess(SocketConnection connection, string token);
		 void SignupFail(SocketConnection connection, int reason);
	}
}