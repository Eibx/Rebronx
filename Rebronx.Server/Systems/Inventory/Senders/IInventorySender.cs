namespace Rebronx.Server.Systems.Inventory.Senders
{
    public interface IInventorySender
    {
        void SendInventory(Player player);
    }
}