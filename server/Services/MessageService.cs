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
        private readonly IPositionRepository positionRepository;

        private readonly ISocketRepository socketRepository;

        public MessageService(IWebSocketCore webSocketCore, IUserRepository playerRepository, IPositionRepository positionRepository, ISocketRepository socketRepository)
        {
            this.webSocketCore = webSocketCore;
            this.playerRepository = playerRepository;
            this.positionRepository = positionRepository;
            this.socketRepository = socketRepository;
        }

        public void Send<T>(ClientConnection connection, string component, string type, T data)
        {
            string json = string.Empty;

            try
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new LowercaseContractResolver();
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);
            }
            catch {}

            var stream = connection.Stream;

            if (stream != null)
                webSocketCore.Send(stream, json);
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

            if (connection?.Stream != null)
                webSocketCore.Send(connection.Stream, json);
        }

        public void SendPosition<T>(int node, string component, string type, T data)
        {
            string json = string.Empty;

            try
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new LowercaseContractResolver();
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { component = component, type = type, data = data }, Formatting.None, settings);
            }
            catch {}


            foreach(var player in positionRepository.GetPlayersByPosition(node))
            {
                var connection = socketRepository.GetConnection(player.Id);

                if (connection?.Stream != null)
                    webSocketCore.Send(connection.Stream, json);
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
                var stream = connection?.Stream;

                if (stream != null)
                    webSocketCore.Send(stream, json);
            }
        }
    }
}