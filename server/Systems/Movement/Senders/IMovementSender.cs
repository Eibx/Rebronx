namespace Rebronx.Server.Systems.Movement.Senders
{
    public interface IMovementSender
    {
        void StartMove(Player player, long moveTime);
        void SetPosition(Player player, int newNode);
    }
}