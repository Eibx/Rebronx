using Rebronx.Server.Models;

namespace Rebronx.Server.DataSenders.Interfaces
{
	public interface ILoginSender
	{
		 void Login(SocketConnection connection, bool loginSuccess);
	}
}