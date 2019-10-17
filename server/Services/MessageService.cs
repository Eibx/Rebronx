using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;

namespace Rebronx.Server.Services
{
    public class MessageService : IMessageService
    {
        private readonly IWebSocketCore _webSocketCore;
        private readonly IPositionRepository _positionRepository;

        private readonly ISocketRepository _socketRepository;

        public MessageService(IWebSocketCore webSocketCore, IPositionRepository positionRepository,
            ISocketRepository socketRepository)
        {
            _webSocketCore = webSocketCore;
            _positionRepository = positionRepository;
            _socketRepository = socketRepository;
        }

        public void Send<T>(ClientConnection connection, string component, string type, T data)
        {
            var json = string.Empty;

            try
            {
                var settings = new JsonSerializerSettings {ContractResolver = new LowercaseContractResolver()};
                json = JsonConvert.SerializeObject(new {component, type, data}, Formatting.None, settings);
            }
            catch
            {
                // ignored
            }

            var stream = connection.Stream;

            if (stream != null)
                _webSocketCore.Send(stream, json);
        }

        public void Send<T>(Player player, string component, string type, T data)
        {
            var json = string.Empty;

            try
            {
                var settings = new JsonSerializerSettings {ContractResolver = new LowercaseContractResolver()};
                json = JsonConvert.SerializeObject(new {component, type, data}, Formatting.None, settings);
            }
            catch
            {
                // ignored
            }

            var connection = _socketRepository.GetConnection(player.Id);

            if (connection?.Stream != null)
                _webSocketCore.Send(connection.Stream, json);
        }

        public void SendPosition<T>(int node, string component, string type, T data)
        {
            var json = string.Empty;

            try
            {
                var settings = new JsonSerializerSettings {ContractResolver = new LowercaseContractResolver()};
                json = JsonConvert.SerializeObject(new {component, type, data}, Formatting.None, settings);
            }
            catch
            {
                // ignored
            }


            foreach (var player in _positionRepository.GetPlayersByPosition(node))
            {
                var connection = _socketRepository.GetConnection(player.Id);

                if (connection?.Stream != null)
                    _webSocketCore.Send(connection.Stream, json);
            }
        }

        public void SendAll<T>(string component, string type, T data)
        {
            var json = string.Empty;

            try
            {
                var settings = new JsonSerializerSettings {ContractResolver = new LowercaseContractResolver()};
                json = JsonConvert.SerializeObject(new {component, type, data}, Formatting.None, settings);
            }
            catch
            {
                // ignored
            }

            foreach (var connection in _socketRepository.GetAllConnections())
            {
                var stream = connection?.Stream;

                if (stream != null)
                    _webSocketCore.Send(stream, json);
            }
        }
    }
}