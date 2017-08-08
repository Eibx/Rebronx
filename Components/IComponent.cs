using System.Collections.Generic;

namespace Rebronx.Server.Components
{
	public interface IComponent
	{
		void Run(IList<Message> messages);
	}
}