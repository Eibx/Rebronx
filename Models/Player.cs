using Rebronx.Server.Models;

public class Player
{
	public string Name { get; set; }
	public Position Position { get; set; }
	public int Health { get; set; }

	public SocketConnection Socket { get; set; }
}