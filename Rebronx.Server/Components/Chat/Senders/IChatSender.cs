namespace Rebronx.Server.Components.Chat.Senders
{
    public interface IChatSender
    {
        void Say(Player player, string message);
    }
}