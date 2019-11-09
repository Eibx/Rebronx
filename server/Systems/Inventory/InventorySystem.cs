using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Systems.Inventory.Services;

namespace Rebronx.Server.Systems.Inventory
{
    public class InventorySystem : System, IInventorySystem
    {
        private readonly IInventoryService _inventoryService;

        public InventorySystem(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemNames.Inventory))
            {
                if (message.Type == "reorder")
                {
                    ProcessReorderRequest(message);
                }
                else if (message.Type == "equip")
                {
                    ProcessEquiptRequest(message);
                }
                else if (message.Type == "unequip")
                {
                    ProcessUnequipRequest(message);
                }
            }
        }

        private void ProcessReorderRequest(Message message)
        {
            var inputMessage = GetData<ReorderInventoryRequest>(message);

            if (inputMessage != null && message?.Player != null)
            {
                _inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
            }
        }

        private void ProcessEquiptRequest(Message message)
        {
            var inputMessage = GetData<EquipItemRequest>(message);

            if (inputMessage != null && message?.Player != null)
            {
                _inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
            }
        }

        private void ProcessUnequipRequest(Message message)
        {
            var inputMessage = GetData<EquipItemRequest>(message);

            if (inputMessage != null && message?.Player != null)
            {
                _inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
            }
        }
    }

    public class ReorderInventoryRequest
    {
        public int From { get; set; }
        public int To { get; set; }
    }

    public class EquipItemRequest
    {
        public int From { get; set; }
        public int? To { get; set; }
    }
}