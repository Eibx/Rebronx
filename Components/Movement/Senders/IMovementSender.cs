namespace Rebronx.Server.Components.Movement.Senders
{
	public interface IMovementSender
	{
		void StartMove(Player player, int newPosition, long moveTime);
		void SetPosition(Player player, int newPosition);
	}
}