using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Enums;

namespace Rebronx.Server.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private Dictionary<int, Item> items;

        public ItemRepository()
        {
            items = new Dictionary<int, Item>();

            DataResult<Item> output = JsonConvert.DeserializeObject<DataResult<Item>>(File.ReadAllText("../Rebronx.Data/items.json"));

            foreach (var item in output.Data)
            {
                items.Add(item.Id, item);
            }
        }

        public Item GetItem(int id)
        {
            return items.ContainsKey(id) ? items[id] : null;
        }

        public List<EquipmentSlot> GetEquipmentSlots(int itemId)
        {
            var item = GetItem(itemId);

            if (item == null) {
                return new List<EquipmentSlot>();
            }

            if (item.Type == "weapon") {
                return new List<EquipmentSlot> { EquipmentSlot.PrimaryWeapon, EquipmentSlot.SecondaryWeapon };
            } else if (item.Type == "ammo") {
                return new List<EquipmentSlot> { EquipmentSlot.PrimaryAmmo, EquipmentSlot.SecondaryAmmo };
            } else if (item.Type == "headarmour") {
                return new List<EquipmentSlot> { EquipmentSlot.HeadArmour };
            } else if (item.Type == "bodyarmour") {
                return new List<EquipmentSlot> { EquipmentSlot.BodyArmour };
            } else if (item.Type == "legarmour") {
                return new List<EquipmentSlot> { EquipmentSlot.LegArmour };
            }

            return new List<EquipmentSlot>();
        }
    }
}