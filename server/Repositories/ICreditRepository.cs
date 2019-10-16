namespace Rebronx.Server.Repositories
{
    public interface ICreditRepository
    {
         void GiveCredit(Player player, int credits);
         void TakeCredit(Player player, int credits);
         void TransferCredit(Player fromPlayer, Player toPlayer, int credits);

         long GetCredits(Player player);
    }
}