using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Systems.Inventory.Services;

namespace Rebronx.Server.Systems.Command
{
    public class CommandSystem : System, ICommandSystem
    {
        private const string Component = "command";
        private readonly IInventoryService _inventoryService;

        public CommandSystem(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "give")
                    MessageSay(message);
            }
        }

        public void MessageSay(Message message)
        {
            var inputMessage = GetData<InputCommandMessage>(message);

            if (inputMessage != null)
            {
                if (!int.TryParse(inputMessage.Arguments[0], out var itemId))
                    return;

                _inventoryService.AddItem(message.Player.Id, itemId, 1);
            }
        }
    }

    public class InputCommandMessage
    {
        public List<string> Arguments { get; set; }
    }
}