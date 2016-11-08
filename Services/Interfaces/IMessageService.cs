using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Services.Interfaces 
{
	public interface IMessageService
	{
		void Send<T>(Player player, string component, string type, T data);
		void SendPosition<T>(Position position, string component, string type, T data);
		void SendAll<T>(string component, string type, T data);
	}
}