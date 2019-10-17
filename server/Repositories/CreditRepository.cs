using System;
using Rebronx.Server.Services;

namespace Rebronx.Server.Repositories
{
    public class CreditRepository : ICreditRepository
    {
        private readonly IDatabaseService _databaseService;

        public CreditRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public long GetCredits(Player player)
        {
            //var database = databaseService.GetConnection();
            return (int)100;//database.HashGet($"player:{player.Id}", "credits");
        }

        public void GiveCredit(Player player, int credits)
        {
            //var database = databaseService.GetConnection();
            //database.HashIncrement($"player:{player.Id}", "credits", credits);
        }

        public void TakeCredit(Player player, int credits)
        {
            //var database = databaseService.GetConnection();
            //database.HashDecrement($"player:{player.Id}", "credits", credits);
        }

        public void TransferCredit(Player fromPlayer, Player toPlayer, int credits)
        {
            //var database = databaseService.GetConnection();
            //database.HashDecrement($"player:{fromPlayer.Id}", "credits", credits);
            //database.HashIncrement($"player:{toPlayer.Id}", "credits", credits);
        }
    }
}