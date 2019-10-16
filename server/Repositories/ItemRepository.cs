using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Enums;

namespace Rebronx.Server.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private Dictionary<int, Item> items;

        public ItemRepository()
        {
            items = new Dictionary<int, Item>();

            //DataResult<Item> output = JsonConvert.DeserializeObject<DataResult<Item>>(File.ReadAllText("../data/items.json"));

            //foreach (var item in output.Data)
            //{
            //    items.Add(item.Id, item);
            //}
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

            switch (item.Type)
            {
                case "weapon":
                    return new List<EquipmentSlot> { EquipmentSlot.PrimaryWeapon, EquipmentSlot.SecondaryWeapon };
                case "ammo":
                    return new List<EquipmentSlot> { EquipmentSlot.PrimaryAmmo, EquipmentSlot.SecondaryAmmo };
                case "headarmor":
                    return new List<EquipmentSlot> { EquipmentSlot.HeadArmour };
                case "bodyarmor":
                    return new List<EquipmentSlot> { EquipmentSlot.BodyArmour };
                case "legarmor":
                    return new List<EquipmentSlot> { EquipmentSlot.LegArmour };
                default:
                    return new List<EquipmentSlot>();
            }
        }
    }
}