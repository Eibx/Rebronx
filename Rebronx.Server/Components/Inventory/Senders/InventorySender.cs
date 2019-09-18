using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory.Repositories;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Inventory.Senders
{
    public class InventorySender : IInventorySender
    {
        private readonly IMessageService messageService;
        private readonly IInventoryRepository inventoryRepository;

        public InventorySender(IMessageService messageService, IInventoryRepository inventoryRepository)
        {
            this.messageService = messageService;
            this.inventoryRepository = inventoryRepository;
        }

        public void SendInventory(Player player)
        {
            var inventory = inventoryRepository.GetInventory(player.Id);

            List<List<int>> data = inventory.Select(x => new List<int> { x.Id, x.Count, x.Slot }).ToList();

            messageService.Send(player, "inventory", "update", data);
        }
    }
}