using System.Text.Json;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        public void Send<T>(ClientConnection connection, string system, string type, T data)
        {
            var json = Serialize(system, type, data);

            var stream = connection.Stream;

            if (stream != null)
                _webSocketCore.Send(stream, json);
        }

        public void Send<T>(Player player, string system, string type, T data)
        {
            var json = Serialize(system, type, data);

            var connection = _socketRepository.GetConnection(player.Id);

            if (connection?.Stream != null)
                _webSocketCore.Send(connection.Stream, json);
        }

        public void SendPosition<T>(int node, string system, string type, T data)
        {
            var json = Serialize(system, type, data);

            foreach (var player in _positionRepository.GetPlayersByPosition(node))
            {
                var connection = _socketRepository.GetConnection(player.Id);

                if (connection?.Stream != null)
                    _webSocketCore.Send(connection.Stream, json);
            }
        }

        public void SendAll<T>(string system, string type, T data)
        {
            var json = Serialize(system, type, data);

            foreach (var connection in _socketRepository.GetAllConnections())
            {
                var stream = connection?.Stream;

                if (stream != null)
                    _webSocketCore.Send(stream, json);
            }
        }

        private string Serialize<T>(string system, string type, T data)
        {
            var json = string.Empty;

            try
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                json = JsonSerializer.Serialize(new { component = system, type, data }, options);
            }
            catch
            {
                // ignored
            }

            return json;
        }
    }
}