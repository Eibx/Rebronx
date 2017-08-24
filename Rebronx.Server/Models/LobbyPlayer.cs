using System;
using Rebronx.Server.Models;

public class LobbyPlayer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Health { get; set; }

	public LobbyPlayer(Player player)
	{
		Random rand = new Random();

		Id = player.Id;
		Name = player.Name;
		Health = player.Health;
	}
}