using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory.Services;

namespace Rebronx.Server.Components.Command
{
    public class CommandComponent : Component, ICommandComponent
    {
        private const string Component = "command";
        private readonly IInventoryService inventoryService;

        public CommandComponent(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
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

                inventoryService.AddItem(message.Player.Id, itemId, 1);
            }
        }
    }

    public class InputCommandMessage
    {
        public List<string> Arguments { get; set; }
    }
}