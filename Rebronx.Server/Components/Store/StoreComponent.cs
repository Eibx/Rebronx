using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory.Services;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Components.Store
{
    public class StoreComponent : Component, IStoreComponent
    {
        private const string Component = "store";
        private readonly IInventoryService inventoryService;
        private readonly IItemRepository itemRepository;

        public StoreComponent(IInventoryService inventoryService, IItemRepository itemRepository)
        {
            this.inventoryService = inventoryService;
            this.itemRepository = itemRepository;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "buy")
                    MessageBuy(message);
            }
        }

        public void MessageBuy(Message message)
        {
            var inputMessage = GetData<InputBuyMessage>(message);

            //TODO: Check if store has item
            //TODO: Get store price and deduct user
            var item = itemRepository.GetItem(inputMessage.Item);

            if (item != null) {
                inventoryService.AddItem(message.Player.Id, inputMessage.Item, inputMessage.Amount);
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