using System;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Models;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.DataSenders
{
	public class LoginSender : ILoginSender
	{
		private readonly IMessageService messageService;

		public LoginSender(IMessageService messageService)
		{
			this.messageService = messageService;
		}

		public void Login(SocketConnection connection, bool loginSuccess)
		{
			var loginMessage = new SendLoginMessage();
			loginMessage.Success = loginSuccess;

			messageService.Send(connection, "login", "login", loginMessage);
		}
	}

	public class SendLoginMessage
	{
		public bool Success { get; set; }
	}
}