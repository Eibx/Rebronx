namespace Rebronx.Server.Components.Movement.Senders
{
	public interface IMovementSender
	{
		void StartMove(Player player, Position newPosition, long moveTime);
		void SetPosition(Player player, Position newPosition);
	}
}