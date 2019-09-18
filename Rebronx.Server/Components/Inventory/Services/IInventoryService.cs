namespace Rebronx.Server.Components.Inventory.Services
{
    public interface IInventoryService
    {
        void MoveItem(int playerId, int from, int? to);
        void AddItem(int playerId, int itemId, int count = 1);
    }
}