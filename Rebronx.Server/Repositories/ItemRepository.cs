using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;

namespace Rebronx.Server.Repositories
{
	public class ItemRepository : IItemRepository
	{
		private Dictionary<int, Item> items;

		public ItemRepository()
		{
			DataResult<Item> output = JsonConvert.DeserializeObject<DataResult<Item>>(File.ReadAllText("items.json"));

			foreach (var item in output.Data)
			{
				items.Add(item.Id, item);
			}
		}

		public Item GetItem(int id)
		{
			return items.ContainsKey(id) ? items[id] : null;
		}
	}
}