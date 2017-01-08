using System;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.DataSenders
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
	}

	public class SendLoginMessage
	{
		public bool Success { get; set; }
		public int Reason { get; set; }
		public string Token { get; set; }
	}
}