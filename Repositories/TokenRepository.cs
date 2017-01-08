using System;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Repositories
{
    public class TokenRepository : ITokenRepository
    {
		private readonly IDatabaseService databaseService;

		public TokenRepository(IDatabaseService databaseService)
		{
			this.databaseService = databaseService;
		}

        public int? GetPlayerId(string token)
        {
			var database = databaseService.GetDatabase();
			var playerId = (int?)database.StringGet($"token:{token}");

			return playerId;
        }

        public string GetToken(Player player)
        {
			var database = databaseService.GetDatabase();
			var token = database.HashGet($"player:{player.Id}", "token");

			return token.ToString();
        }

        public void SetPlayerToken(Player player, string token)
        {
            var database = databaseService.GetDatabase();
			var oldToken = GetToken(player);

			database.KeyDelete($"token:{oldToken}");
			
			database.HashSet($"player:{player.Id}", "token", token);
			database.StringSet($"token:{token}", player.Id);
        }

		public void RemovePlayerToken(Player player)
        {
            var database = databaseService.GetDatabase();
			var token = GetToken(player);

			if (token != null) 
			{
				database.HashDelete($"player:{player.Id}", "token");	
			}

			database.KeyDelete($"token:{token}");
        }
    }
}