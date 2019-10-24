using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Repositories;

namespace Rebronx.Server.Services
{
    public class WebSocketCore : IWebSocketCore
    {
        private readonly ISocketRepository _socketRepository;
        private readonly List<ConnectingClient> _connectingClients;
        private readonly TcpListener _server;
        private readonly X509Certificate _serverCertificate;

        private const int TextFrame = 129;
        private const int CloseFrame = 136;

        public WebSocketCore(ISocketRepository socketRepository)
        {
            _socketRepository = socketRepository;

            _serverCertificate = new X509Certificate2(System.Configuration.ConfigurationManager.AppSettings["CertificatePath"], System.Configuration.ConfigurationManager.AppSettings["CertificatePassword"], X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

            _server = new TcpListener(IPAddress.Any, 21220);
            _server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _server.Start();

            _connectingClients = new List<ConnectingClient>();
        }

        public void GetNewConnections()
        {
            while (_server.Pending())
            {
                var client = _server.AcceptTcpClient();

                SslStream sslStream = new SslStream(client.GetStream(), true);
                sslStream.AuthenticateAsServer(_serverCertificate, false, SslProtocols.Tls13, true);

                _connectingClients.Add(new ConnectingClient()
                {
                    TcpClient = client,
                    Stream = sslStream,
                    Connected = DateTime.Now
                });
            }

            HandleHttpConnection();
        }

        public List<WebSocketMessage> PollMessages()
        {
            List<WebSocketMessage> output = new List<WebSocketMessage>();

            var sockets = _socketRepository.GetAllConnections();

            for (int i = 0; i < sockets.Count; i++)
            {
                var socket = sockets[i];

                List<byte> data = new List<byte>();

                while (socket.TcpClient.Client.Poll(1000, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[1024];

                    var received = socket.Stream.Read(buffer, 0, buffer.Length);

                    if (received == 0)
                        break;

                    data.AddRange(buffer.Take(received));
                }

                if (socket.IsTimedOut())
                {
                    Console.WriteLine("Dead connection (" + socket.Id + ")");
                    sockets.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!data.Any())
                    continue;

                if (data[0] != TextFrame)
                    continue;

                ulong size = 0;
                var payloadIndex = 0;
                var mask = new byte[4];

                if (data[1] <= 253)
                {
                    size = (data[1] - 128u);
                    mask = data.Skip(2).Take(4).ToArray();
                    payloadIndex = 6;
                }
                else if (data[1] == 254)
                {
                    size = BitConverter.ToUInt16(data.ToArray(), 2);
                    mask = data.Skip(4).Take(4).ToArray();
                    payloadIndex = 8;
                }
                else if (data[1] == 255)
                {
                    size = BitConverter.ToUInt64(data.ToArray(), 2);
                    mask = data.Skip(10).Take(4).ToArray();
                    payloadIndex = 14;
                }

                var payload = data.Skip(payloadIndex).Take((int)size).ToArray();
                for (var j = 0; j < payload.Length; j++)
                {
                    payload[j] = (byte)(payload[j] ^ mask[j % 4]);
                }

                var jsonData = Encoding.ASCII.GetString(payload, 0, payload.Count());
                if (jsonData == "ping")
                {
                    socket.LastMessage = DateTime.Now;
                    Send(socket.Stream, "pong");
                    continue;
                }

                WebSocketMessage wsMessage = null;
                try
                {
                    wsMessage = JsonConvert.DeserializeObject<WebSocketMessage>(jsonData);
                }
                catch (Exception)
                {
                    // ignored
                }

                if (wsMessage != null && wsMessage.HasData())
                {
                    wsMessage.Connection = socket;
                    output.Add(wsMessage);
                }
            }

            return output;
        }

        public void Send(SslStream stream, string data)
        {
            var bytes = new List<byte> { TextFrame };

            var dataBytes = Encoding.UTF8.GetBytes(data).ToList();

            if (dataBytes.Count <= 125)
            {
                bytes.Add((byte)dataBytes.Count);
                bytes.AddRange(dataBytes);
            }
            else if (dataBytes.Count <= 65535)
            {
                bytes.Add(126);
                bytes.Add((byte)((dataBytes.Count() >> 8) & 255));
                bytes.Add((byte)((dataBytes.Count()) & 255));
                bytes.AddRange(dataBytes);
            }
            else
            {
                bytes.Add(127);
                bytes.Add((byte)((dataBytes.Count() >> 56) & 255));
                bytes.Add((byte)((dataBytes.Count() >> 48) & 255));
                bytes.Add((byte)((dataBytes.Count() >> 40) & 255));
                bytes.Add((byte)((dataBytes.Count() >> 32) & 255));
                bytes.Add((byte)((dataBytes.Count() >> 24) & 255));
                bytes.Add((byte)((dataBytes.Count() >> 16) & 255));
                bytes.Add((byte)((dataBytes.Count() >> 8) & 255));
                bytes.Add((byte)((dataBytes.Count()) & 255));
                bytes.AddRange(dataBytes);
            }

            try
            {
                stream.Write(bytes.ToArray());
            }
            catch (SocketException e)
            {
                // Broken pipe exception can happen here
                Console.WriteLine(e.ToString());
            }
        }

        private void SendClose(Socket socket, int reason)
        {
            var bytes = new List<byte>
            {
                CloseFrame,

                //length
                2,

                //code/reason 4001 - TODO: Send Reason
                15,
                161
            };

            socket.Send(bytes.ToArray());
        }

        private void HandleHttpConnection()
        {
            for (var i = 0; i < _connectingClients.Count; i++)
            {
                var connection = _connectingClients[i];

                var httpRequest = GetHttpRequest(connection);

                if (connection.IsTimeout() || !connection.TcpClient.Connected || httpRequest == null)
                {
                    _connectingClients.RemoveAt(i);
                    i--;
                    continue;
                }

                var httpHeaders = GetHttpHeaders(httpRequest);

                if (httpHeaders.ContainsKey("Sec-WebSocket-Key"))
                {
                    var responseBytes = CreateConnectionResponse(httpHeaders);

                    connection.Stream.Write(responseBytes, 0, responseBytes.Length);

                    var clientConnection = new ClientConnection()
                    {
                        Id = Guid.NewGuid(),
                        TcpClient = connection.TcpClient,
                        Stream = connection.Stream,
                        LastMessage = DateTime.Now
                    };

                    _socketRepository.AddUnauthorizedConnection(clientConnection);
                }

                if (i <= _connectingClients.Count - 1)
                {
                    _connectingClients.RemoveAt(i);
                    i--;
                }
            }
        }

        private static string GetHttpRequest(ConnectingClient connectingClient)
        {
            var output = string.Empty;

            while (connectingClient.TcpClient.Client.Poll(1000, SelectMode.SelectRead))
            {
                if (!connectingClient.TcpClient.Connected)
                    return null;

                var buffer = new byte[1024];
                var received = 0;

                try
                {
                    received = connectingClient.Stream.Read(buffer, 0, buffer.Length);
                }
                catch (System.IO.IOException)
                {
                    // ignored
                }

                if (received == 0)
                    break;

                var message = Encoding.ASCII.GetString(buffer, 0, received);
                output += message;
            }

            return output;
        }

        private static byte[] CreateConnectionResponse(Dictionary<string, string> headers)
        {
            var webSocketKey = headers["Sec-WebSocket-Key"];

            var keyHash = Hash(webSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
            var wsKeyResult = Convert.ToBase64String(keyHash);

            var response = "HTTP/1.1 101 Switching Protocols\r\n" +
                           "Upgrade: websocket\r\n" +
                           "Connection: Upgrade\r\n" +
                           "Sec-WebSocket-Accept: " + wsKeyResult + "\r\n\r\n";
            var responseBytes = Encoding.ASCII.GetBytes(response);
            return responseBytes;
        }

        private static Dictionary<string, string> GetHttpHeaders(string data)
        {
            var headers = new Dictionary<string, string>();

            if (!data.StartsWith("GET"))
                return headers;

            var lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("GET /"))
                {
                    var token = GetQueryString(line, "u");

                    if (token != null)
                        headers.Add("Username", token);

                    continue;
                }

                var splitIndex = line.IndexOf(":", StringComparison.Ordinal);
                if (splitIndex > -1)
                {
                    var propertyKey = line.Substring(0, splitIndex);
                    var propertyValue = line.Substring(splitIndex + 1);
                    headers.Add(propertyKey, propertyValue.Trim());
                }
            }

            return headers;
        }

        private static byte[] Hash(string input)
        {
            using var sha1 = SHA1.Create();

            return sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private static string GetQueryString(string line, string name)
        {
            return line.Split(' ')?.Skip(1)?.Take(1).FirstOrDefault()?.Split('?').LastOrDefault()?.Split('&').FirstOrDefault(x => x.StartsWith(name))?.Split('=').LastOrDefault();
        }
    }

    public class ConnectingClient
    {
        public TcpClient TcpClient { get; set; }

        public SslStream Stream { get; set; }
        public DateTime Connected { get; set; }

        public bool IsTimeout()
        {
            return (Connected.AddSeconds(5) < DateTime.Now);
        }
    }

    public class WebSocketMessage
    {
        public ClientConnection Connection { get; set; }
        public string Component { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }

        public bool HasData()
        {
            return !string.IsNullOrEmpty(Component) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Data);
        }
    }
}