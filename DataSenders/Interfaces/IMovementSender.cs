namespace Rebronx.Server.DataSenders.Interfaces
{
    public interface IMovementSender
    {
         void Move(Player player, Position fromPosition, Position toPosition);
    }
}