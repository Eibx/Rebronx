using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Inventory.Services;

namespace Rebronx.Server.Systems.Store
{
    public class StoreSystem : System, IStoreSystem
    {
        private readonly IInventoryService _inventoryService;
        private readonly IItemRepository _itemRepository;

        public StoreSystem(IInventoryService inventoryService, IItemRepository itemRepository)
        {
            _inventoryService = inventoryService;
            _itemRepository = itemRepository;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemTypes.Store))
            {

            }
        }

        public void ProcessBuyRequest(Message message)
        {
            var inputMessage = GetData<InputBuyMessage>(message);

            //TODO: Check if store has item
            //TODO: Get store price and deduct user
            var item = _itemRepository.GetItem(inputMessage.Item);

            if (item != null) {
                _inventoryService.AddItem(message.Player.Id, inputMessage.Item, inputMessage.Amount);
            }
        }
    }

    public class InputBuyMessage
    {
        public int Shop { get; set; }
        public int Item { get; set; }
        public int Amount { get; set; }
    }
}