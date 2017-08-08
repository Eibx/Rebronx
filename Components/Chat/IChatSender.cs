namespace Rebronx.Server.Components.Chat
{
    public interface IChatSender
    {
         void Say(Player player, string message);
    }
}