using System.Collections.Generic;
using Rebronx.Server.Enums;
using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories
{
    public interface IItemRepository
    {
        Item GetItem(int id);
        List<EquipmentSlot> GetEquipmentSlots(int itemId);
    }
}