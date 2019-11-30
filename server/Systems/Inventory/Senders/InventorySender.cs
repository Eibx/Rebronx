using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Inventory.Repositories;

namespace Rebronx.Server.Systems.Inventory.Senders
{
    public class InventorySender : IInventorySender
    {
        private readonly IMessageService _messageService;
        private readonly IInventoryRepository _inventoryRepository;

        public InventorySender(IMessageService messageService, IInventoryRepository inventoryRepository)
        {
            _messageService = messageService;
            _inventoryRepository = inventoryRepository;
        }

        public void SendInventory(Player player)
        {
            var inventory = _inventoryRepository.GetInventory(player.Id);

            List<List<int>> data = inventory.Select(x => new List<int> { x.Id, x.Count, x.Slot }).ToList();

            //_messageService.Send(player, SystemTypes.Inventory, , data);
        }
    }
}