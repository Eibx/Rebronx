using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory;
using Rebronx.Server.Components.Inventory.Repositories;
using Rebronx.Server.Components.Inventory.Senders;
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
			}
		}

		public void ReorderInventory(Message message)
		{
			var inputMessage = GetData<ReorderInventoryMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				if (inputMessage.Current < 0 || inputMessage.Current > 17 || inputMessage.New < 0 || inputMessage.New > 17) {
					return;
				}

				inventoryRepository.ReorderInventory(message.Player.Id, inputMessage.Current, inputMessage.New);

				inventorySender.SendInventory(message.Player);
			}
		}

		public void EquipItem(Message message)
		{
			var inputMessage = GetData<EquipItemMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				var inventory = inventoryRepository.GetInventory(message.Player.Id);
				var inventoryItem = inventory.FirstOrDefault(x => x.InventoryPosition == inputMessage.From);

				if (inventoryItem == null) 
				{
					return;
				}

				int equipmentSlot = inputMessage.To ?? GetFreeEquipmentSlot(inventoryItem.Id, inventory);

				inventoryRepository.EquipItem(message.Player.Id, inputMessage.From, equipmentSlot);

				inventorySender.SendInventory(message.Player);
			}
		}

		public void UnequipItem(Message message)
		{
			var inputMessage = GetData<EquipItemMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				var inventory = inventoryRepository.GetInventory(message.Player.Id);

				if (inventory.Count(x => x.InventoryPosition.HasValue) >= 18)
				{
					return;
				}

				var moveItemTo = inputMessage.To ?? GetFreeInventoryIndex(inventory);

				if (moveItemTo < 0 || moveItemTo >= 18)
				{
					return;
				}

				inventoryRepository.UnequipItem(message.Player.Id, inputMessage.From, moveItemTo);

				inventorySender.SendInventory(message.Player);
			}
		}

		private int GetFreeInventoryIndex(List<Models.InventoryItem> inventory) 
		{
			var ordered = inventory.Where(x => x.InventoryPosition.HasValue).OrderBy(x => x.InventoryPosition);

			if (ordered.Count() >= 18) {
				return -1;
			}

			for (int i = 0; i < 18; i++)
			{
				if (!ordered.Any(x => x.InventoryPosition == i))
				{
					return i;
				}
			}

			return -1;
		}

		private int GetFreeEquipmentSlot(int itemId, List<Models.InventoryItem> inventory) 
		{
			var equipmentSlot = -1;
			var equipmentSlots = itemRepository.GetEquipmentSlots(itemId);

			foreach (var slot in equipmentSlots)
			{
				if (!inventory.Any(x => x.EquipmentPosition.HasValue && x.EquipmentPosition.Value == ((int)slot)))
				{
					equipmentSlot = (int)slot;
					break;
				}
			}

			return equipmentSlot;
		}
	}

	public class ReorderInventoryMessage
	{
		public int Current { get; set; }
		public int New { get; set; }
	}

	public class EquipItemMessage
	{
		public int From { get; set; }
		public int? To { get; set; }
	}
}