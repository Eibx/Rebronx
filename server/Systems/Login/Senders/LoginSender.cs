using System;
using Rebronx.Server.Enums;
using Rebronx.Server.Models;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Login.Senders
{
    public class LoginSender : ILoginSender
    {
        private readonly IMessageService _messageService;

        public LoginSender(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void Success(Player player, string token)
        {
            var loginMessage = new LoginResponse();
            loginMessage.Success = true;
            loginMessage.Reason = 0;
            loginMessage.Token = token;

            _messageService.Send(player, SystemTypes.Login, SystemTypes.LoginTypes.Login, loginMessage);
        }

        public void Fail(Player player, int reason)
        {
            var loginMessage = new LoginResponse();
            loginMessage.Success = false;
            loginMessage.Reason = 0;
            loginMessage.Token = null;

            _messageService.Send(player, SystemTypes.Login, SystemTypes.LoginTypes.Login, loginMessage);
        }

        public void Fail(ClientConnection connection, int reason)
        {
            var loginMessage = new LoginResponse();
            loginMessage.Success = false;
            loginMessage.Reason = reason;
            loginMessage.Token = null;

            _messageService.Send(connection, SystemTypes.Login, SystemTypes.LoginTypes.Login, loginMessage);
        }

        public void SignupSuccess(ClientConnection connection, string token)
        {
            var signupMessage = new SignupResponse();
            signupMessage.Success = true;
            signupMessage.Token = token;
            signupMessage.Reason = 0;

            _messageService.Send(connection, SystemTypes.Login, SystemTypes.LoginTypes.Signup, signupMessage);
        }

        public void SignupFail(ClientConnection connection, int reason)
        {
            var signupMessage = new SignupResponse();
            signupMessage.Success = true;
            signupMessage.Token = null;
            signupMessage.Reason = reason;

            _messageService.Send(connection, SystemTypes.Login, SystemTypes.LoginTypes.Signup, signupMessage);
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public int Reason { get; set; }
        public string Token { get; set; }
    }

    public class SignupResponse
    {
        public bool Success { get; set; }
        public int Reason { get; set; }
        public string Token { get; set; }
    }
}