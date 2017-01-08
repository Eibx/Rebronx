using System.Linq;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Services 
{
	public class MessageService : IMessageService
	{
		private readonly IWebSocketCore webSocketCore;
		private readonly IUserRepository playerRepository;

		private readonly ISocketRepository socketRepository;

		public MessageService(IWebSocketCore webSocketCore, IUserRepository playerRepository, ISocketRepository socketRepository)
		{
			this.webSocketCore = webSocketCore;
			this.playerRepository = playerRepository;
			this.socketRepository = socketRepository;
		}

		public void Send<T>(SocketConnection connection, string component, string type, T data) 
		{
			string json = string.Empty;
			
			try
			{
				var settings = new JsonSerializerSettings();
				settings.ContractResolver = new LowercaseContractResolver();
				json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
			} 
			catch {}
					
			var socket = connection.Socket;

			if (socket != null)
				webSocketCore.Send(socket, json);
		}

		public void Send<T>(Player player, string component, string type, T data) 
		{
			string json = string.Empty;
			
			try
			{
				var settings = new JsonSerializerSettings();
				settings.ContractResolver = new LowercaseContractResolver();
				json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
			} 
			catch {}
					
			var connection = socketRepository.GetConnection(player.Id);

			if (connection?.Socket != null)
				webSocketCore.Send(connection.Socket, json);
		}

		public void SendPosition<T>(Position position, string component, string type, T data) 
		{
			string json = string.Empty;
			
			try
			{
				var settings = new JsonSerializerSettings();
				settings.ContractResolver = new LowercaseContractResolver();
				json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
			} 
			catch {}


			foreach(var player in playerRepository.GetPlayersByPosition(position)) 
			{
				var connection = socketRepository.GetConnection(player.Id);

				if (connection?.Socket != null)
					webSocketCore.Send(connection.Socket, json);
			}
		}

		public void SendAll<T>(string component, string type, T data) 
		{
			string json = string.Empty;
			
			try
			{
				var settings = new JsonSerializerSettings();
				settings.ContractResolver = new LowercaseContractResolver();
				json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);	
			} 
			catch {}

			foreach(var connection in socketRepository.GetAllConnections()) 
			{
				var socket = connection?.Socket;

				if (socket != null)
					webSocketCore.Send(socket, json);
			}
		}
	}
}