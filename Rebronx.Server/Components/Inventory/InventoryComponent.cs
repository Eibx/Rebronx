using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory;
using Rebronx.Server.Components.Inventory.Repositories;
using Rebronx.Server.Components.Inventory.Senders;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Components.Inventory
{
	public class InventoryComponent : Component, IInventoryComponent
	{
		private const string Component = "inventory";

		private readonly IInventoryRepository inventoryRepository;
		private readonly IItemRepository itemRepository;
		private readonly IInventorySender inventorySender;

		public InventoryComponent(IInventoryRepository inventoryRepository, IItemRepository itemRepository, IInventorySender inventorySender)
		{
			this.inventoryRepository = inventoryRepository;
			this.itemRepository = itemRepository;
			this.inventorySender = inventorySender;
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
				if (!IsValidSlot(inputMessage.From) || !IsValidSlot(inputMessage.To))
					return;

				var inventory = inventoryRepository.GetInventory(message.Player.Id);
				var inventoryItem = inventory.FirstOrDefault(x => x.Slot == inputMessage.From);

				if (inventoryItem == null)
					return;

				inventoryRepository.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);

				inventorySender.SendInventory(message.Player);
			}
		}

		public void EquipItem(Message message)
		{
			var inputMessage = GetData<EquipItemMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				var inventory = inventoryRepository.GetInventory(message.Player.Id);
				var inventoryItem = inventory.FirstOrDefault(x => x.Slot == inputMessage.From);

				if (inventoryItem == null) 
				{
					return;
				}

				int equipmentSlot = inputMessage.To ?? GetFreeEquipmentSlot(inventoryItem.Id, inventory);

				inventoryRepository.MoveItem(message.Player.Id, inputMessage.From, equipmentSlot);

				inventorySender.SendInventory(message.Player);
			}
		}

		public void UnequipItem(Message message)
		{
			var inputMessage = GetData<EquipItemMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				var inventory = inventoryRepository.GetInventory(message.Player.Id);

				if (inventory.Count(x => x.Slot > 100) >= 18)
				{
					return;
				}

				var moveItemTo = inputMessage.To ?? GetFreeInventorySlot(inventory);

				if (!IsValidSlot(moveItemTo))
				{
					return;
				}

				inventoryRepository.MoveItem(message.Player.Id, inputMessage.From, moveItemTo);

				inventorySender.SendInventory(message.Player);
			}
		}

		private int GetFreeInventorySlot(List<Models.InventoryItem> inventory) 
		{
			var ordered = inventory.OrderBy(x => x.Slot);

			if (ordered.Count() >= 18) {
				return -1;
			}

			for (int i = 0; i < 18; i++)
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

		private bool IsValidSlot(int slot) {
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