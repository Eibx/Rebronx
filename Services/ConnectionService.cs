using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Services
{
	public class ConnectionService : IConnectionService
	{
		private readonly IUserRepository playerRepository;
		private readonly ISocketRepository socketRepository;
		private readonly ITokenRepository tokenRepository;
		private readonly IJoinSender joinSender;
		private readonly ILoginSender loginSender;
		private readonly ILobbySender lobbySender;

		public ConnectionService(IUserRepository playerRepository, ISocketRepository socketRepository, ITokenRepository tokenRepository, IJoinSender joinSender, ILoginSender loginSender, ILobbySender lobbySender)
		{
			this.socketRepository = socketRepository;
			this.playerRepository = playerRepository;
			this.tokenRepository = tokenRepository;
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

				Player player = null;
				var playerId = socketRepository.GetPlayerId(message.Connection.Id);

				if (playerId != null)
					player = playerRepository.GetPlayerById(playerId.Value);

				if (player == null)
					return output;

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
			
			Player player = null;
			string token;
			
			if (!string.IsNullOrEmpty(loginData.Token)) 
			{
				player = playerRepository.GetPlayerByToken(loginData.Token);

				if (player == null) {
					loginSender.Fail(loginMessage.Connection, 4002);
					return;
				}

				token = loginData.Token;
			}
			else
			{
				player = playerRepository.GetPlayerByLogin(loginData.Username, loginData.Password);

				if (player == null) {
					loginSender.Fail(loginMessage.Connection, 4001);
					return;
				}

				var bytes = new byte[32];
				var rnd = RandomNumberGenerator.Create();
				rnd.GetBytes(bytes);
				token = Convert.ToBase64String(bytes);
				tokenRepository.SetPlayerToken(player, token);
			}

			//TODO
			//Send information about player
			//Send map information
			//Send lobby information
			//Maybe just make a "JoinSender" that does all that.

			socketRepository.AddConnection(player.Id, loginMessage.Connection);

			loginSender.Success(player, token);
			joinSender.Join(player);
		}

		public void HandleDeadPlayers()
		{
			var connections = socketRepository.GetAllConnections();
			var timeouts = connections.Where(x => x.Socket == null || x.IsTimedout()).ToList();
			foreach (var timeout in timeouts)
			{
				Console.WriteLine($"Socket removed: {timeout.Id} - {timeout.LastMessage}");
				socketRepository.RemoveConnection(timeout.Id);
			}
		}
	}
}

public class LoginMessage 
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string Token { get; set; }
}