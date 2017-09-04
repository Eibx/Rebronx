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
		private const string Component = "combat";

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
				var inventory = inventoryRepository.GetInventory(message.Player.Id);

				inventorySender.SendInventory(message?.Player);
			}
		}


	}

	public class ReorderInventoryMessage
	{
		public int Current { get; set; }
		public int New { get; set; }
	}
}