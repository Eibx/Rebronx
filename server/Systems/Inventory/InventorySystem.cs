using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Systems.Inventory;
using Rebronx.Server.Systems.Inventory.Repositories;
using Rebronx.Server.Systems.Inventory.Senders;
using Rebronx.Server.Enums;
using Rebronx.Server.Systems.Inventory.Services;

namespace Rebronx.Server.Systems.Inventory
{
    public class InventorySystem : System, IInventorySystem
    {
        private const string Component = "inventory";

        private readonly IInventoryService _inventoryService;

        public InventorySystem(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "reorder")
                {
                    ReorderInventory(message);
                }
                else if (message.Type == "equip")
                {
                    EquipItem(message);
                }
                else if (message.Type == "unequip")
                {
                    UnequipItem(message);
                }
            }
        }

        public void ReorderInventory(Message message)
        {
            var inputMessage = GetData<ReorderInventoryMessage>(message);

            if (inputMessage != null && message?.Player != null)
            {
                _inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
            }
        }

        public void EquipItem(Message message)
        {
            var inputMessage = GetData<EquipItemMessage>(message);

            if (inputMessage != null && message?.Player != null)
            {
                _inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
            }
        }

        public void UnequipItem(Message message)
        {
            var inputMessage = GetData<EquipItemMessage>(message);

            if (inputMessage != null && message?.Player != null)
            {
                _inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
            }
        }
    }

    public class ReorderInventoryMessage
    {
        public int From { get; set; }
        public int To { get; set; }
    }

    public class EquipItemMessage
    {
        public int From { get; set; }
        public int? To { get; set; }
    }
}