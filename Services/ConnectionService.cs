using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Rebronx.Server.DataSenders.Interfaces;
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

			//TODO
			//Send information about player
			//Send map information
			//Send lobby information
			//Maybe just make a "JoinSender" that does all that.

			socketRepository.AddConnection(player.Id, loginMessage.Connection);

			loginSender.Success(player, token);
			joinSender.Join(player);
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

			if (!string.IsNullOrEmpty(signupData.Username) && !string.IsNullOrEmpty(signupData.Password)) 
			{
				if (signupData.Username.Length > 3) 
				{
					var username = signupData.Username;

					if (playerRepository.GetPlayerByUsername(username) == null) 
					{
						var hash = PBKDF2.HashPassword(signupData.Password);
						var previousToken = string.Empty;
						var token = string.Empty;

						// TODO: Handle if it still - for some reason - hasn't found an available token
						do 
						{
							previousToken = token;
							token = GenerateToken();

							if (token == previousToken)
								throw new Exception("Same Token generated twice! Shouldn't statistically happen");
						} 
						while (playerRepository.GetPlayerByToken(token) != null);
						
						playerRepository.CreateNewPlayer(username, hash, token);
						loginSender.SignupSuccess(signupMessage.Connection, token);
					} 
					else 
					{
						reason = 1003;
					}
				} 
				else 
				{
					reason = 1002;
				}
			} 
			else
			{
				reason = 1001;
			}
			
			loginSender.SignupFail(signupMessage.Connection, reason);
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