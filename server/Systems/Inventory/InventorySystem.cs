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
            foreach (var message in messages.Where(m => m.System == SystemTypes.Inventory))
            {

            }
        }

        private void ProcessReorderRequest(Message message)
        {

        }

        private void ProcessEquiptRequest(Message message)
        {

        }

        private void ProcessUnequipRequest(Message message)
        {

        }
    }
}