using Rebronx.Server.Models;

public class LobbyPlayer
{
	public string Name { get; set; }
	public int Health { get; set; }

	public LobbyPlayer(Player player)
	{
		Name = player.Name;
		Health = player.Health;
	}
}