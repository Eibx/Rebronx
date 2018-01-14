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

		public void SetPlayerPositon(Player player, Position position)
		{
			databaseService.ExecuteNonQuery(
				"UPDATE players SET x = @x, y = @y WHERE id = @id",
				new Dictionary<string, object>() {
					{ "id", player.Id },
					{ "x", position.X },
					{ "y", position.Y }
				});
		}

		public List<Player> GetPlayersByPosition(Position position)
		{
			var data = databaseService.ExecuteReader(
				"SELECT * FROM players WHERE x = @x AND y = @y",
				new Dictionary<string, object>() {
					{ "x", position.X },
					{ "y", position.Y }
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
				Position = new Position(record.GetInt32(record.GetOrdinal("x")), record.GetInt32(record.GetOrdinal("y"))),
				Health = record.GetInt32(record.GetOrdinal("health")),
			};
		}
	}
}
