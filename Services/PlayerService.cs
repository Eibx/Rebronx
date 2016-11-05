using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Rebronx.Server.Models;

public class PlayerService : IPlayerService
{
	private IWebSocketCore webSocketCore;
	private Dictionary<string, Player> players;

	public PlayerService(IWebSocketCore webSocketCore)
	{
		this.webSocketCore = webSocketCore;

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

	public void Send(Player player, string component, string type, string data) 
	{
		string json = string.Empty;
		
		try
		{
			var settings = new JsonSerializerSettings();
			settings.ContractResolver = new LowercaseContractResolver();
			json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
		} 
		catch {}
				
		var socket = player?.Socket?.Socket;

		if (socket != null)
        	webSocketCore.Send(socket, json);
	}

	public void SendPosition<T>(Position position, string component, string type, T data) 
	{
		string json = string.Empty;
		
		try
		{
			var settings = new JsonSerializerSettings();
			settings.ContractResolver = new LowercaseContractResolver();
			json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
		} 
		catch {}

		foreach(var player in players.Select(x => x.Value).Where(x => x.Position.Equals(position))) 
		{
			var socket = player?.Socket?.Socket;

			if (socket != null)
				webSocketCore.Send(socket, json);
		}
	}

	public void SendAll<T>(string component, string type, T data) 
	{
		string json = string.Empty;
		
		try
		{
			var settings = new JsonSerializerSettings();
			settings.ContractResolver = new LowercaseContractResolver();
			json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
		} 
		catch {}

		foreach(var player in players.Select(x => x.Value)) 
		{
			var socket = player?.Socket?.Socket;

			if (socket != null)
				webSocketCore.Send(socket, json);
		}
	}
}