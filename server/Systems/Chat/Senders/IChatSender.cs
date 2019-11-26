namespace Rebronx.Server.Systems.Chat.Senders
{
    public interface IChatSender
    {
        void Execute();
        void Say(Player player, string message);
    }
}