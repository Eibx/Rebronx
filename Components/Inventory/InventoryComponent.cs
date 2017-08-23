using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Components.Inventory;
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
				if (message.Type == "get")
					GetInventory(message);
				else if (message.Type == "reorder")
					ReorderInventory(message);
			}
		}

		public void GetInventory(Message message) 
		{
			if (message?.Player != null)
			{
				
			}
		}

		public void ReorderInventory(Message message) 
		{
			var inputMessage = GetData<ReorderInventoryMessage>(message);
			
			if (inputMessage != null && message?.Player != null)
			{
				//var inventory = inventoryRepository.GetInventory(message.Player.Id);

				
			}
		}


	}

	public class ReorderInventoryMessage 
	{
		public List<int?> InventoryItems { get; set; }
	}
}