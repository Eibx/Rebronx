using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Systems.Inventory.Services;

namespace Rebronx.Server.Systems.Command
{
    public class CommandSystem : System, ICommandSystem
    {
        private readonly IInventoryService _inventoryService;

        public CommandSystem(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemNames.Command))
            {
                if (message.Type == "give")
                    ProcessGiveRequest(message);
            }
        }

        public void ProcessGiveRequest(Message message)
        {
            var inputMessage = GetData<CommandRequest>(message);

            if (inputMessage != null)
            {
                if (!int.TryParse(inputMessage.Arguments[0], out var itemId))
                    return;

                _inventoryService.AddItem(message.Player.Id, itemId, 1);
            }
        }
    }

    public class CommandRequest
    {
        public List<string> Arguments { get; set; }
    }
}