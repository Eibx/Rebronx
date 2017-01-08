using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;
using StackExchange.Redis;

namespace Rebronx.Server.Repositories
{
    public class MovementRepository : IMovementRepository
    {
		private readonly IDatabaseService databaseService;

		public MovementRepository(IDatabaseService databaseService)
		{
			this.databaseService = databaseService;
		}

        public void SetPlayerPositon(Player player, Position position) 
		{
			var database = databaseService.GetDatabase();

			

			database.HashSet($"player:{player.Id}", "pos.x", position.X);
			database.HashSet($"player:{player.Id}", "pos.y", position.Y);
			database.HashSet($"player:{player.Id}", "pos.z", position.Z);
		}
    }
}