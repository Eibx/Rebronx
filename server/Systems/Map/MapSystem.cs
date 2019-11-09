using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Map
{
    public class MapSystem : IMapSystem
    {
        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemNames.Map))
            {
                if (message.Type == "data")
                    ProcessDataRequest(message);
            }
        }

        private void ProcessDataRequest(Message message)
        {
            Console.WriteLine(message.Data);
        }
    }
}