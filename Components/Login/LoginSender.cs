using System;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Login
{
	public class LoginSender : ILoginSender
	{
		private readonly IMessageService messageService;
		private readonly ISocketRepository socketRepository;

		public LoginSender(IMessageService messageService, ISocketRepository socketRepository)
		{
			this.messageService = messageService;
			this.socketRepository = socketRepository;
		}

		public void Success(Player player, string token)
		{
			var loginMessage = new SendLoginMessage();
			loginMessage.Success = true;
			loginMessage.Reason = 0;
			loginMessage.Token = token;

			messageService.Send(player, "login", "login", loginMessage);
		}

		public void Fail(Player player, int reason)
		{
			var loginMessage = new SendLoginMessage();
			loginMessage.Success = false;
			loginMessage.Reason = 0;
			loginMessage.Token = null;

			messageService.Send(player, "login", "login", loginMessage);
		}

		public void Fail(SocketConnection connection, int reason)
		{
			var loginMessage = new SendLoginMessage();
			loginMessage.Success = false;
			loginMessage.Reason = reason;
			loginMessage.Token = null;

			messageService.Send(connection, "login", "login", loginMessage);
		}

		public void SignupSuccess(SocketConnection connection, string token)
		{
			var signupMessage = new SendSignupMessage();
			signupMessage.Success = true;
			signupMessage.Token = token;
			signupMessage.Reason = 0;

			messageService.Send(connection, "login", "signup", signupMessage);
		}

		public void SignupFail(SocketConnection connection, int reason)
		{
			var signupMessage = new SendSignupMessage();
			signupMessage.Success = true;
			signupMessage.Token = null;
			signupMessage.Reason = reason;

			messageService.Send(connection, "login", "signup", signupMessage);
		}
	}

	public class SendLoginMessage
	{
		public bool Success { get; set; }
		public int Reason { get; set; }
		public string Token { get; set; }
	}

	public class SendSignupMessage
	{
		public bool Success { get; set; }
		public int Reason { get; set; }
		public string Token { get; set; }
	}
}