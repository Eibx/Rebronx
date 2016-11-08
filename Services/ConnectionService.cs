using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Services
{
	public class ConnectionService : IConnectionService
	{
		private readonly IPlayerRepository playerRepository;
		private readonly IJoinSender joinSender;
		private readonly ILobbySender lobbySender;

		public ConnectionService(IPlayerRepository playerRepository, IJoinSender joinSender, ILobbySender lobbySender)
		{
			this.playerRepository = playerRepository;
			this.joinSender = joinSender;
			this.lobbySender = lobbySender;
		}

		public List<Message> ConvertToMessages(List<WebSocketMessage> messages)
		{
			var output = new List<Message>();

			foreach (var message in messages)
			{
				var token = message?.Socket?.Token;
				if (token == null)
					continue;

				var player = playerRepository.GetPlayer(token);

				output.Add(new Message()
				{
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

				Player player = playerRepository.GetPlayer(connection.Token);
				if (player == null)
				{
					player = new Player();
					player.Health = 1000;
					player.Name = connection.Token;
					player.Position = new Position() { X = 0, Y = 0, Z = 0 };
				}

				player.Socket = connection;

				playerRepository.AddPlayer(connection.Token, player);

				//Send informatino about player
				//Send map information
				//Send lobby information
				//Maybe just make a "JoinSender" that does all that.
				joinSender.Join(player);
			}
		}

		public void HandleDeadPlayers()
		{
			var players = playerRepository.GetPlayers();
			var timeouts = players.Where(x => x.Value.Socket == null || x.Value.Socket.IsTimedout()).ToList();
			foreach (var timeout in timeouts)
			{
				Console.WriteLine("Deleted player - " + timeout.Value.Name);
				playerRepository.RemovePlayer(timeout.Key);

				lobbySender.Update(timeout.Value.Position);
			}
		}
	}
}