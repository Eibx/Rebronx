using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Repositories
{
	public class PositionRepository : IPositionRepository
	{
		private Dictionary<string, List<int>> positions;

		private readonly IDatabaseService databaseService;
		private readonly ISocketRepository socketRepository;

		public PositionRepository(IDatabaseService databaseService, ISocketRepository socketRepository)
		{
			positions = new Dictionary<string, List<int>>();

			this.databaseService = databaseService;
			this.socketRepository = socketRepository;
		}

		public void SetPlayerPositon(Player player, int position)
		{
			databaseService.ExecuteNonQuery(
				"UPDATE players SET position = @position WHERE id = @id",
				new Dictionary<string, object>() {
					{ "id", player.Id },
					{ "position", position }
				});
		}

		public List<Player> GetPlayersByPosition(int position)
		{
			var data = databaseService.ExecuteReader(
				"SELECT * FROM players WHERE position = @position",
				new Dictionary<string, object>() {
					{ "position", position },
				});
			
			var output = new List<Player>();
			while (data.Read()) {
				output.Add(TransformPlayer(data));
			}

			data.Close();

			return output.Where(x => socketRepository.IsPlayerOnline(x.Id)).ToList();
		}

		private Player TransformPlayer(IDataRecord record) {
			return new Player() {
				Id = record.GetInt32(record.GetOrdinal("id")),
				Name = record.GetString(record.GetOrdinal("name")),
				Position = record.GetInt32(record.GetOrdinal("position")),
				Health = record.GetInt32(record.GetOrdinal("health")),
			};
		}
	}
}
