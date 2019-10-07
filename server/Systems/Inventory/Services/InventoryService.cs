using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Systems.Inventory.Repositories;
using Rebronx.Server.Systems.Inventory.Senders;

namespace Rebronx.Server.Systems.Inventory.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly IInventorySender inventorySender;
        private readonly IItemRepository itemRepository;

        private readonly IUserRepository userRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IInventorySender inventorySender,
            IItemRepository itemRepository,
            IUserRepository userRepository)
        {
            this.inventoryRepository = inventoryRepository;
            this.inventorySender = inventorySender;
            this.itemRepository = itemRepository;
            this.userRepository = userRepository;
        }

        public void AddItem(int playerId, int itemId, int count = 1)
        {
            var inventory = inventoryRepository.GetInventory(playerId);
            var freeSlot = GetFreeInventorySlot(inventory);

            if (freeSlot > -1)
            {
                inventoryRepository.AddItem(playerId, itemId, count, freeSlot);
            }

            var player = userRepository.GetPlayerById(playerId);

            if (player != null)
                inventorySender.SendInventory(player);
        }

        public void MoveItem(int playerId, int from, int? to)
        {
            if (!IsValidSlot(from) || (to.HasValue && !IsValidSlot(to)))
                return;

            int destinationSlot = -1;
            var inventory = inventoryRepository.GetInventory(playerId);
            var inventoryItem = inventory.FirstOrDefault(x => x.Slot == from);

            if (inventoryItem == null)
                return;

            if (to != null)
            {
                destinationSlot = to.Value;
            }
            else if (to == null && from < 100)
            {
                destinationSlot = GetFreeInventorySlot(inventory);
            }
            else if (to == null && from >= 100)
            {
                destinationSlot = GetFreeEquipmentSlot(inventoryItem.Id, inventory);
            }

            if (destinationSlot == -1)
                return;

            if (!IsValidSlotForItem(inventoryItem.Id, destinationSlot))
                return;

            var destinationItem = inventory.FirstOrDefault(x => x.Slot == destinationSlot);

            if (destinationItem != null)
            {
                if (IsValidSlotForItem(destinationItem.Id, from))
                {
                    inventoryRepository.SwapItem(playerId, destinationSlot, from);
                }
            }
            else
            {
                inventoryRepository.MoveItem(playerId, from, destinationSlot);
            }

            var player = userRepository.GetPlayerById(playerId);

            if (player != null)
                inventorySender.SendInventory(player);
        }

        private bool IsValidSlot(int? slot) {
            if (!slot.HasValue)
            {
                return false;
            }
            if (slot >= 100 && slot <= 117)
            {
                return true;
            }
            else if (Enum.IsDefined(typeof(EquipmentSlot), slot))
            {
                return true;
            }

            return false;
        }

        private int GetFreeInventorySlot(List<Models.InventoryItem> inventory)
        {
            var ordered = inventory.Where(x => x.Slot >= 100).OrderBy(x => x.Slot);

            if (ordered.Count() >= 18) {
                return -1;
            }

            for (int i = 100; i < 118; i++)
            {
                if (!ordered.Any(x => x.Slot == i))
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetFreeEquipmentSlot(int itemId, List<Models.InventoryItem> inventory)
        {
            var freeSlot = -1;
            var equipmentSlots = itemRepository.GetEquipmentSlots(itemId);

            foreach (var slot in equipmentSlots)
            {
                if (!inventory.Any(x => x.Slot == (int)slot))
                {
                    freeSlot = (int)slot;
                    break;
                }
            }

            return freeSlot;
        }

        private bool IsValidSlotForItem(int itemId, int slot)
        {
            if (slot < 100) {
                var slots = itemRepository.GetEquipmentSlots(itemId).Select(x => (int)x);

                return slots.Contains(slot);
            }

            return true;
        }
    }
}