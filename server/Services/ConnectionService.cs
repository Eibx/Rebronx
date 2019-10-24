using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Rebronx.Server.Helpers;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Join;
using Rebronx.Server.Systems.Join.Senders;
using Rebronx.Server.Systems.Lobby;
using Rebronx.Server.Systems.Lobby.Senders;
using Rebronx.Server.Systems.Login;
using Rebronx.Server.Systems.Login.Senders;

namespace Rebronx.Server.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IUserRepository _playerRepository;
        private readonly ISocketRepository _socketRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IPositionRepository _movementRepository;
        private readonly IJoinSender _joinSender;
        private readonly ILoginSender _loginSender;
        private readonly ILobbySender _lobbySender;

        public ConnectionService(
            IUserRepository playerRepository,
            ISocketRepository socketRepository,
            ITokenRepository tokenRepository,
            IPositionRepository movementRepository,
            IJoinSender joinSender,
            ILoginSender loginSender,
            ILobbySender lobbySender)
        {
            _socketRepository = socketRepository;
            _playerRepository = playerRepository;
            _tokenRepository = tokenRepository;
            _movementRepository = movementRepository;
            _joinSender = joinSender;
            _loginSender = loginSender;
            _lobbySender = lobbySender;
        }

        public List<Message> ConvertToMessages(List<WebSocketMessage> messages)
        {
            var output = new List<Message>();

            foreach (var message in messages)
            {
                switch (message.Type)
                {
                    case "login":
                        HandleLoginMessage(message);
                        continue;

                    case "signup":
                        HandleSignupMessage(message);
                        continue;
                }

                Player player = null;
                var playerId = _socketRepository.GetPlayerId(message.Connection.Id);

                if (playerId != null)
                    player = _playerRepository.GetPlayerById(playerId.Value);

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
            try
            {
                loginData = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginMessage>(loginMessage.Data);
            } catch { }

            if (loginData == null)
                return;

            Player player;
            string token;

            if (!string.IsNullOrEmpty(loginData.Token))
            {
                player = _playerRepository.GetPlayerByToken(loginData.Token);

                if (player == null) {
                    _loginSender.Fail(loginMessage.Connection, 4002);
                    return;
                }

                token = loginData.Token;
            }
            else
            {
                player = _playerRepository.GetPlayerByLogin(loginData.Username, loginData.Password);

                if (player == null) {
                    _loginSender.Fail(loginMessage.Connection, 4001);
                    return;
                }

                token = GenerateToken();
                _tokenRepository.SetPlayerToken(player, token);
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

            if (_playerRepository.GetPlayerByName(signupData.Username) != null)
                reason = 1003;

            if (reason > 0)
            {
                _loginSender.SignupFail(signupMessage.Connection, reason);
                return;
            }

            var name = signupData.Username;
            var hash = Pbkdf2.HashPassword(signupData.Password);
            var previousToken = string.Empty;
            var token = string.Empty;

            do
            {
                previousToken = token;
                token = GenerateToken();

                if (token == previousToken)
                    throw new Exception("Same Token generated twice! Shouldn't statistically happen");
            }
            while (_playerRepository.GetPlayerByToken(token) != null);

            _playerRepository.CreateNewPlayer(name, hash, token);

            var player = _playerRepository.GetPlayerByToken(token);

            SendPlayerInformation(player, signupMessage.Connection, token);
        }

        public void HandleDeadPlayers()
        {
            var connections = _socketRepository.GetAllConnections();
            var timeouts = connections.Where(x => x.TcpClient == null || x.IsTimedOut()).ToList();

            foreach (var timeout in timeouts)
            {
                Player player = null;
                var playerId = _socketRepository.GetPlayerId(timeout.Id);
                if (playerId.HasValue)
                {
                    player = _playerRepository.GetPlayerById(playerId.Value);
                }

                Console.WriteLine($"Connection removed: {timeout.Id} - {timeout.LastMessage}");
                _socketRepository.RemoveConnection(timeout.Id);

                if (player != null)
                {
                    _lobbySender.Update(player.Node);
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

            _socketRepository.AddConnection(player.Id, connection);
            _movementRepository.SetPlayerPosition(player, player.Node);

            _loginSender.Success(player, token);
            _joinSender.Join(player);
        }

        private string GenerateToken() {
            var bytes = new byte[32];
            var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
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
}