namespace Rebronx.Server.Systems.Chat.Senders
{
    public interface IChatSender
    {
        void Say(Player player, string message);
    }
}