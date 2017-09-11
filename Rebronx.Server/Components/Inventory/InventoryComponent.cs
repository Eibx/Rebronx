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
		private readonly IInventorySender inventorySender;

		public InventoryComponent(IInventoryRepository inventoryRepository, IInventorySender inventorySender)
		{
			this.inventoryRepository = inventoryRepository;
			this.inventorySender = inventorySender;
		}

		public void Run(IList<Message> messages)
		{
			foreach (var message in messages.Where(m => m.Component == Component))
			{
				if (message.Type == "reorder")
					ReorderInventory(message);
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


	}

	public class ReorderInventoryMessage
	{
		public int Current { get; set; }
		public int New { get; set; }
	}
}