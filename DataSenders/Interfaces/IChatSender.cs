namespace Rebronx.Server.DataSenders.Interfaces
{
    public interface IChatSender
    {
         void Say(Player player, string message);
    }
}