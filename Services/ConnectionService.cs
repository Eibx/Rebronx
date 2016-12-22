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
		private readonly ISocketRepository socketRepository;
		private readonly IJoinSender joinSender;
		private readonly ILoginSender loginSender;
		private readonly ILobbySender lobbySender;

		public ConnectionService(IPlayerRepository playerRepository, ISocketRepository socketRepository, IJoinSender joinSender, ILoginSender loginSender, ILobbySender lobbySender)
		{
			this.socketRepository = socketRepository;

			this.playerRepository = playerRepository;
			this.joinSender = joinSender;
			this.loginSender = loginSender;
			this.lobbySender = lobbySender;
		}

		public List<Message> ConvertToMessages(List<WebSocketMessage> messages)
		{
			var output = new List<Message>();

			foreach (var message in messages)
			{
				if (message.Type == "login") {
					HandleLoginMessage(message);
					continue;
				}

				var player = playerRepository.GetPlayerByConnection(message.Connection.Id);

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

		public void HandleLoginMessage(WebSocketMessage loginMessage)
		{
			LoginMessage loginData = null;
			try {
				loginData = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginMessage>(loginMessage.Data);
			} catch { }

			if (loginData == null)
				return;
			
			if (!string.IsNullOrEmpty(loginData.Username) && loginData.Password == "1") 
			{
				var player = playerRepository.GetPlayerByUsername(loginData.Username);

				//TODO
				//Send informatino about player
				//Send map information
				//Send lobby information
				//Maybe just make a "JoinSender" that does all that.
				loginSender.Login(loginMessage.Connection, true);
				joinSender.Join(player);
			} 
			else 
			{
				loginSender.Login(loginMessage.Connection, false);
				return;
			}
		}

		public void HandleDeadPlayers()
		{
			var connections = socketRepository.GetAllConnections();
			var timeouts = connections.Where(x => x.Socket == null || x.IsTimedout()).ToList();
			foreach (var timeout in timeouts)
			{
				var player = playerRepository.GetPlayerByConnection(timeout.Id);

				if (player != null) {
					Console.WriteLine("Removing player (dead) - " + player.Name);
					playerRepository.RemovePlayer(player.Id);

					lobbySender.Update(player.Position);
				} else {
					//TODO: add logging
					Console.WriteLine($"Socket was timed out but player was null - ID:{timeout.Id}");
				}
			}
		}
	}
}

public class LoginMessage 
{
	public string Username { get; set; }
	public string Password { get; set; }
}