using System.Linq;
using Newtonsoft.Json;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Services 
{
	public class MessageService : IMessageService
	{
		private readonly IWebSocketCore webSocketCore;
		private readonly IPlayerRepository playerRepository;

		public MessageService(IWebSocketCore webSocketCore, IPlayerRepository playerRepository)
		{
			this.webSocketCore = webSocketCore;
			this.playerRepository = playerRepository;
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
					
			var socket = player?.Socket?.Socket;

			if (socket != null)
				webSocketCore.Send(socket, json);
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


			foreach(var player in playerRepository.GetPlayers().Select(x => x.Value).Where(x => x.Position.Equals(position))) 
			{
				var socket = player?.Socket?.Socket;

				if (socket != null)
					webSocketCore.Send(socket, json);
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

			foreach(var player in playerRepository.GetPlayers().Select(x => x.Value)) 
			{
				var socket = player?.Socket?.Socket;

				if (socket != null)
					webSocketCore.Send(socket, json);
			}
		}
	}
}