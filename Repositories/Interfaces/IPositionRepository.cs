using System.Collections.Generic;

namespace Rebronx.Server.Repositories.Interfaces
{
	public interface IPositionRepository
	{
		void SetPlayerPositon(Player player, int position);
		List<Player> GetPlayersByPosition(int position);
	}
}