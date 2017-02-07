namespace Rebronx.Server.DataSenders.Interfaces
{
    public interface ICombatSender
    {
		void AttackerReport(Player player, int damage);
		void VictimReport(Player player, int damage);
    }
}