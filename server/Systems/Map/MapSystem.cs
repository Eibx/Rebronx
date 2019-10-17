using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Map
{
    public class MapSystem : IMapSystem
    {
        private const string Component = "map";
        private readonly IWebSocketCore _webSocketCore;

        public MapSystem(IWebSocketCore webSocketCore)
        {
            _webSocketCore = webSocketCore;
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