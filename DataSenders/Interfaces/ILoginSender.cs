using Rebronx.Server.Models;

namespace Rebronx.Server.DataSenders.Interfaces
{
	public interface ILoginSender
	{
		 void Success(Player player, string token);
		 void Fail(SocketConnection connection, int reason);
		 void Fail(Player player, int reason);
	}
}