namespace Rebronx.Server.Systems.Combat.Senders
{
    public interface ICombatSender
    {
        void AttackerReport(Player player, int damage);
        void VictimReport(Player player, int damage);
    }
}