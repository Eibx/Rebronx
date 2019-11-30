using System;
using System.Collections.Generic;
using System.IO;
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

        private const byte TextFrame = 129;
        private const byte BinaryFrame = 130;
        private const byte CloseFrame = 136;
        private const byte PingFrame = 137;
        private const byte PongFrame = 138;

        public WebSocketCore(ISocketRepository socketRepository)
        {
            _socketRepository = socketRepository;

            var certPath = System.Configuration.ConfigurationManager.AppSettings["CertificatePath"];
            var certExportPassword = System.Configuration.ConfigurationManager.AppSettings["CertificatePassword"];
            _serverCertificate = new X509Certificate2(certPath, certExportPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

            _server = new TcpListener(IPAddress.Any, 21220);
            _server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _server.Start();

            _connectingClients = new List<ConnectingClient>();
        }

        public void GetNewConnections()
        {
            int newConnections = 0;

            while (_server.Pending())
            {
                var client = _server.AcceptTcpClient();
                client.Client.NoDelay = true;

                var sslStream = new SslStream(client.GetStream(), true);
                sslStream.AuthenticateAsServer(_serverCertificate, false, SslProtocols.Tls13, false);

                _connectingClients.Add(new ConnectingClient()
                {
                    TcpClient = client,
                    Stream = sslStream,
                    Connected = DateTime.Now
                });

                newConnections++;

                if (newConnections > 10)
                    break;
            }

            HandleHttpConnection();
        }

        public List<WebSocketMessage> PollMessages()
        {
            var output = new List<WebSocketMessage>();

            var sockets = _socketRepository.GetAllConnections();

            for (var i = 0; i < sockets.Count; i++)
            {
                var socket = sockets[i];

                var data = new List<byte>();

                while (socket.TcpClient.Client.Poll(10, SelectMode.SelectRead))
                {
                    var buffer = new byte[1024];

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

                if (data[0] != BinaryFrame)
                    continue;

                ulong size = 0;
                var payloadIndex = 0;
                var mask = new byte[4];

                if (data[1] <= 253)
                {
                    size = (data[1]);
                    mask = data.Skip(2).Take(4).ToArray();
                    payloadIndex = 6;
                }
                else if (data[1] == 254)
                {
                    size = BitConverter.ToUInt16(data.ToArray(), 1);
                    mask = data.Skip(4).Take(4).ToArray();
                    payloadIndex = 8;
                }
                else if (data[1] == 255)
                {
                    size = BitConverter.ToUInt64(data.ToArray(), 1);
                    mask = data.Skip(10).Take(4).ToArray();
                    payloadIndex = 14;
                }

                var payload = data.Skip(payloadIndex).Take((int)size).ToArray();
                for (var j = 0; j < payload.Length; j++)
                {
                    payload[j] = (byte)(payload[j] ^ mask[j % 4]);
                }

                var jsonData = Encoding.UTF8.GetString(payload, 2, payload.Count()-2);

                if (jsonData == "ping")
                {
                    socket.LastMessage = DateTime.Now;
                    continue;
                }

                Console.WriteLine(jsonData);

                WebSocketMessage message = new WebSocketMessage
                {
                    System = payload[0],
                    Type = payload[1],
                    Data = jsonData
                };

                if (message.HasData())
                {
                    message.Connection = socket;
                    output.Add(message);
                }
            }

            return output;
        }

        public void Send(SslStream stream, byte[] dataBytes)
        {
            var bytes = new List<byte> { BinaryFrame };

            if (dataBytes.Length <= 125)
            {
                bytes.Add((byte)dataBytes.Length);
                bytes.AddRange(dataBytes);
            }
            else if (dataBytes.Length <= 65535)
            {
                bytes.Add(126);
                bytes.Add((byte)((dataBytes.Length >> 8) & 255));
                bytes.Add((byte)((dataBytes.Length) & 255));
                bytes.AddRange(dataBytes);
            }
            else
            {
                bytes.Add(127);
                bytes.Add((byte)((dataBytes.Length >> 56) & 255));
                bytes.Add((byte)((dataBytes.Length >> 48) & 255));
                bytes.Add((byte)((dataBytes.Length >> 40) & 255));
                bytes.Add((byte)((dataBytes.Length >> 32) & 255));
                bytes.Add((byte)((dataBytes.Length >> 24) & 255));
                bytes.Add((byte)((dataBytes.Length >> 16) & 255));
                bytes.Add((byte)((dataBytes.Length >> 8) & 255));
                bytes.Add((byte)((dataBytes.Length) & 255));
                bytes.AddRange(dataBytes);
            }

            try
            {
                stream.WriteAsync(bytes.ToArray());
            }
            catch (SocketException e)
            {
                // Broken pipe exception can happen here
                Console.WriteLine(e.ToString());
            }
        }

        private void SendClose(SslStream stream, short reason)
        {
            var reasonBytes = BitConverter.GetBytes(reason);
            var bytes = new byte[]
            {
                CloseFrame, 2, reasonBytes[0], reasonBytes[1]
            };

            stream.WriteAsync(bytes);
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

                    try
                    {
                        connection.Stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    catch (IOException)
                    {
                        // ignore
                    }

                    var clientConnection = new ClientConnection()
                    {
                        Id = Guid.NewGuid(),
                        TcpClient = connection.TcpClient,
                        Stream = connection.Stream,
                        LastMessage = DateTime.Now
                    };

                    _socketRepository.AddUnauthorizedConnection(clientConnection);

                    if (i <= _connectingClients.Count - 1)
                    {
                        _connectingClients.RemoveAt(i);
                        i--;
                    }
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
                catch (IOException)
                {
                    // ignored
                }

                if (received == 0)
                    break;

                var message = Encoding.UTF8.GetString(buffer, 0, received);
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
            var responseBytes = Encoding.UTF8.GetBytes(response);
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
                    continue;

                var splitIndex = line.IndexOf(":", StringComparison.Ordinal);
                if (splitIndex == -1)
                    continue;

                var propertyKey = line.Substring(0, splitIndex);
                var propertyValue = line.Substring(splitIndex + 1);
                headers.Add(propertyKey, propertyValue.Trim());
            }

            return headers;
        }

        private static byte[] Hash(string input)
        {
            using var sha1 = SHA1.Create();
            return sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }

    public class ConnectingClient
    {
        public TcpClient TcpClient { get; set; }

        public SslStream Stream { get; set; }
        public DateTime Connected { get; set; }

        public bool IsTimeout()
        {
            return (Connected.AddSeconds(30) < DateTime.Now);
        }
    }

    public class WebSocketMessage
    {
        public ClientConnection Connection { get; set; }
        public byte System { get; set; }
        public byte Type { get; set; }
        public string Data { get; set; }

        public bool HasData()
        {
            return System > 0 && Type > 0 && !string.IsNullOrEmpty(Data);
        }
    }
}