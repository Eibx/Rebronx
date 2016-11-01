using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Models;

public class PlayerService : IPlayerService
{
	private Dictionary<string, Player> players;

	public PlayerService()
	{
		players = new Dictionary<string, Player>();
	}

    public List<Message> ConvertToMessages(List<WebSocketMessage> messages)
    {
		var output = new List<Message>();

		foreach (var message in messages)
		{
			Player player;
			if (!players.TryGetValue(message.Socket.Token, out player))
				continue;

			output.Add(new Message() {
				Player = player,
				Component = message.Component,
				Type = message.Type,
				Data = message.Data
			});
		}

		return output;
    }

    public void HandleNewPlayers(List<SocketConnection> connections)
    {
		foreach (var connection in connections)
		{
			Player player;
			if (players.TryGetValue(connection.Token, out player))
				continue;

			player = new Player();
			player.Health = 1000;
			player.Name = connection.Token;
			player.Position = new Position() { X = 0, Y = 0, Z = 0 };
			player.Socket = connection;

			players.Add(connection.Token, player);
		}

		foreach(var dead in players.Where(x => x.Value.Socket == null).ToList()) {
			players.Remove(dead.Key);
			Console.WriteLine("Deleted player - " + players[dead.Key].Name);
		}
    }
}