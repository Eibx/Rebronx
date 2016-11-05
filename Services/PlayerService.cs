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
			if (connection.Token == null)
				continue;

			Player player;
			if (!players.TryGetValue(connection.Token, out player)) {
				player = new Player();
			}

			player.Health = 1000;
			player.Name = connection.Token;
			player.Position = new Position() { X = 0, Y = 0, Z = 0 };
			player.Socket = connection;

			if (players.ContainsKey(connection.Token)) {
				players[connection.Token] = player;
			} else {
				players.Add(connection.Token, player);
			}
		}

		foreach(var dead in players.Where(x => x.Value.Socket == null || x.Value.Socket.IsTimedout()).ToList()) {
			Console.WriteLine("Deleted player - " + players[dead.Key].Name);
			players.Remove(dead.Key);
		}
    }
}