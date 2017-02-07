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

        public void SetPlayerPositon(Player player, Position oldPosition, Position newPosition) 
		{
			var database = databaseService.GetDatabase();

			database.HashSet($"player:{player.Id}", "pos.x", newPosition.X);
			database.HashSet($"player:{player.Id}", "pos.y", newPosition.Y);
			database.HashSet($"player:{player.Id}", "pos.z", newPosition.Z);

			database.SetRemove($"position:{oldPosition.X}.{oldPosition.Y}.{oldPosition.Z}", player.Id);
			database.SetAdd($"position:{newPosition.X}.{newPosition.Y}.{newPosition.Z}", player.Id);
		}
    }
}