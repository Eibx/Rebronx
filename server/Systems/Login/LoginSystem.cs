using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Helpers;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Join.Senders;
using Rebronx.Server.Systems.Login.Senders;
using Rebronx.Server.Systems.Movement.Senders;

namespace Rebronx.Server.Systems.Login
{
    public class LoginSystem : System, ILoginSystem
    {
        private const string Component = "login";

        private readonly IUserRepository _userRepository;
        private readonly ILoginSender _loginSender;
        private readonly ITokenRepository _tokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IPositionRepository _positionRepository;
        private readonly IJoinSender _joinSender;
        private readonly ISocketRepository _socketRepository;

        public LoginSystem(
            IUserRepository userRepository,
            ILoginSender loginSender,
            ITokenRepository tokenRepository,
            ITokenService tokenService,
            IJoinSender joinSender,
            ISocketRepository socketRepository,
            IPositionRepository positionRepository)
        {
            _userRepository = userRepository;
            _loginSender = loginSender;
            _tokenRepository = tokenRepository;
            _tokenService = tokenService;
            _joinSender = joinSender;
            _socketRepository = socketRepository;
            _positionRepository = positionRepository;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                switch (message.Type)
                {
                    case "login":
                        Login(message as UnauthorizedMessage);
                        break;
                    case "signup":
                        Signup(message as UnauthorizedMessage);
                        break;
                }
            }
        }

        private void Login(UnauthorizedMessage loginMessage)
        {
            var loginData = GetData<LoginMessage>(loginMessage);

            if (loginData == null)
                return;

            Player player;
            string token;

            if (!string.IsNullOrEmpty(loginData.Token))
            {
                player = _userRepository.GetPlayerByToken(loginData.Token);

                if (player == null)
                {
                    _loginSender.Fail(loginMessage.Connection, 4002);
                    return;
                }

                token = loginData.Token;
            }
            else
            {
                player = _userRepository.GetPlayerByLogin(loginData.Username, loginData.Password);

                if (player == null)
                {
                    _loginSender.Fail(loginMessage.Connection, 4001);
                    return;
                }

                token = _tokenService.GenerateUniqueToken();
                _tokenRepository.SetPlayerToken(player, token);
            }

            SendPlayerInformation(player, loginMessage.Connection, token);
        }

        private void Signup(UnauthorizedMessage signupMessage)
        {
            var signupData = GetData<SignupMessage>(signupMessage);

            if (signupData == null)
                return;

            var reason = 0;

            if (string.IsNullOrEmpty(signupData.Username) || string.IsNullOrEmpty(signupData.Password))
                reason = 1001;

            if (signupData.Username.Length <= 3)
                reason = 1002;

            if (_userRepository.GetPlayerByName(signupData.Username) != null)
                reason = 1003;

            if (reason > 0)
            {
                _loginSender.SignupFail(signupMessage.Connection, reason);
                return;
            }

            var name = signupData.Username;
            var hash = Pbkdf2.HashPassword(signupData.Password);

            var token = _tokenService.GenerateUniqueToken();

            _userRepository.CreateNewPlayer(name, hash, token);

            var player = _userRepository.GetPlayerByToken(token);

            SendPlayerInformation(player, signupMessage.Connection, token);
        }

        private void SendPlayerInformation(Player player, ClientConnection connection, string token)
        {
            _socketRepository.AddConnection(player.Id, connection);
            _positionRepository.SetPlayerPosition(player, player.Node);

            _loginSender.Success(player, token);
            _joinSender.Join(player);
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