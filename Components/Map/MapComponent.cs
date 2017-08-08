using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebronx.Server.Components.Map
{
	public class MapComponent : IMapComponent
	{
		private const string Component = "map";
		private readonly IWebSocketCore webSocketCore;

		public MapComponent(IWebSocketCore webSocketCore)
		{
			this.webSocketCore = webSocketCore;
		}

		public void Run(IList<Message> messages)
		{
			foreach (var message in messages.Where(m => m.Component == Component))
			{
				if (message.Type == "data")
					MessageData(message);
			}
		}

		public void MessageData(Message message)
		{
			Console.WriteLine(message.Data);
		}
	}
}