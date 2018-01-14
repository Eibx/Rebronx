using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Rebronx.Server.Components.Join;
using Rebronx.Server.Components.Join.Senders;
using Rebronx.Server.Components.Lobby;
using Rebronx.Server.Components.Lobby.Senders;
using Rebronx.Server.Components.Login;
using Rebronx.Server.Components.Login.Senders;
using Rebronx.Server.Helpers;
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
		private readonly IPositionRepository movementRepository;
		private readonly IJoinSender joinSender;
		private readonly ILoginSender loginSender;
		private readonly ILobbySender lobbySender;

		public ConnectionService(
			IUserRepository playerRepository, 
			ISocketRepository socketRepository, 
			ITokenRepository tokenRepository, 
			IPositionRepository movementRepository,
			IJoinSender joinSender, 
			ILoginSender loginSender, 
			ILobbySender lobbySender)
		{
			this.socketRepository = socketRepository;
			this.playerRepository = playerRepository;
			this.tokenRepository = tokenRepository;
			this.movementRepository = movementRepository;
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
				} else if (message.Type == "signup") {
					HandleSignupMessage(message);
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

				token = GenerateToken();
				tokenRepository.SetPlayerToken(player, token);
			}

			SendPlayerInformation(player, loginMessage.Connection, token);
		}

		public void HandleSignupMessage(WebSocketMessage signupMessage)
		{
			SignupMessage signupData = null;
			try {
				signupData = Newtonsoft.Json.JsonConvert.DeserializeObject<SignupMessage>(signupMessage.Data);
			} catch { }

			if (signupData == null)
				return;

			int reason = 0;

			if (string.IsNullOrEmpty(signupData.Username) || string.IsNullOrEmpty(signupData.Password)) 
				reason = 1001;

			if (signupData.Username.Length <= 3) 
				reason = 1002;

			if (playerRepository.GetPlayerByName(signupData.Username) != null)
				reason = 1003;

			if (reason > 0) 
			{
				loginSender.SignupFail(signupMessage.Connection, reason);
				return;
			}

			var name = signupData.Username;
			var hash = PBKDF2.HashPassword(signupData.Password);
			var previousToken = string.Empty;
			var token = string.Empty;

			do
			{
				previousToken = token;
				token = GenerateToken();

				if (token == previousToken)
					throw new Exception("Same Token generated twice! Shouldn't statistically happen");
			} 
			while (playerRepository.GetPlayerByToken(token) != null);
			
			playerRepository.CreateNewPlayer(name, hash, token);

			var player = playerRepository.GetPlayerByToken(token);

			SendPlayerInformation(player, signupMessage.Connection, token);
		}

		public void HandleDeadPlayers()
		{
			var connections = socketRepository.GetAllConnections();
			var timeouts = connections.Where(x => x.Client == null || x.IsTimedout()).ToList();
			foreach (var timeout in timeouts)
			{
				Player player = null;
				var playerId = socketRepository.GetPlayerId(timeout.Id);
				if (playerId.HasValue)
				{
					player = playerRepository.GetPlayerById(playerId.Value);					
				}

				Console.WriteLine($"Connection removed:{timeout.Id} - {timeout.LastMessage}");
				socketRepository.RemoveConnection(timeout.Id);

				if (player != null) 
				{
					lobbySender.Update(player.Position);
				}
			}
		}

		private void SendPlayerInformation(Player player, ClientConnection connection, string token) 
		{
			//TODO
			//Send information about player
			//Send map information
			//Send lobby information
			//Maybe just make a "JoinSender" that does all that.

			socketRepository.AddConnection(player.Id, connection);
			movementRepository.SetPlayerPositon(player, player.Position);

			loginSender.Success(player, token);
			joinSender.Join(player);
		}

		private string GenerateToken() {
			var bytes = new byte[32];
			var rnd = RandomNumberGenerator.Create();
			rnd.GetBytes(bytes);
			return Convert.ToBase64String(bytes);
		}
	}
}

public class LoginMessage 
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string Token { get; set; }
}

public class SignupMessage 
{
	public string Username { get; set; }
	public string Password { get; set; }
}