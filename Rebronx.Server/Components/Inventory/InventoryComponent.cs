using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory;
using Rebronx.Server.Components.Inventory.Repositories;
using Rebronx.Server.Components.Inventory.Senders;
using Rebronx.Server.Components.Inventory.Services;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Components.Inventory
{
	public class InventoryComponent : Component, IInventoryComponent
	{
		private const string Component = "inventory";

		private readonly IInventoryService inventoryService;

		public InventoryComponent(IInventoryService inventoryService)
		{
			this.inventoryService = inventoryService;
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
				inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
			}
		}

		public void EquipItem(Message message)
		{
			var inputMessage = GetData<EquipItemMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
			}
		}

		public void UnequipItem(Message message)
		{
			var inputMessage = GetData<EquipItemMessage>(message);

			if (inputMessage != null && message?.Player != null)
			{
				inventoryService.MoveItem(message.Player.Id, inputMessage.From, inputMessage.To);
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