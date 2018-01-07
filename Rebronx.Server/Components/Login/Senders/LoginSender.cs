using System;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Login.Senders
{
	public class LoginSender : ILoginSender
	{
		private readonly IMessageService messageService;

		public LoginSender(IMessageService messageService)
		{
			this.messageService = messageService;
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

		public void Fail(ClientConnection connection, int reason)
		{
			var loginMessage = new SendLoginMessage();
			loginMessage.Success = false;
			loginMessage.Reason = reason;
			loginMessage.Token = null;

			messageService.Send(connection, "login", "login", loginMessage);
		}

		public void SignupSuccess(ClientConnection connection, string token)
		{
			var signupMessage = new SendSignupMessage();
			signupMessage.Success = true;
			signupMessage.Token = token;
			signupMessage.Reason = 0;

			messageService.Send(connection, "login", "signup", signupMessage);
		}

		public void SignupFail(ClientConnection connection, int reason)
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