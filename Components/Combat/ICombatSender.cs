namespace Rebronx.Server.Components.Combat
{
	public interface ICombatSender
	{
		void AttackerReport(Player player, int damage);
		void VictimReport(Player player, int damage);
	}
}