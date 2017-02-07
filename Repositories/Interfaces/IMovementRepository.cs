namespace Rebronx.Server.Repositories.Interfaces
{
    public interface IMovementRepository
    {
         void SetPlayerPositon(Player player, Position oldPosition, Position newPosition);
    }
}