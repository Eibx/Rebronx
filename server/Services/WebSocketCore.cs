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
        private const int TextFrame = 129;
        private const int BinaryFrame = 64;
        List<ConnectingClient> _connectingClients;
        TcpListener _server;
        X509Certificate _serverCertificate;

        public WebSocketCore(ISocketRepository socketRepository)
        {
            _socketRepository = socketRepository;

            _serverCertificate = new X509Certificate2("../rebronx.p12", "1", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 21220);
            _server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _server.Start();

            _connectingClients = new List<ConnectingClient>();
        }

        public void GetNewConnections()
        {
            while (_server.Pending())
            {
                Console.WriteLine("accepting connection?");

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
                var s = sockets[i];

                List<byte> data = new List<byte>();

                while (s.Client.Client.Poll(1000, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[1024];

                    var received = s.Stream.Read(buffer, 0, buffer.Length);

                    if (received == 0)
                        break;

                    data.AddRange(buffer.Take(received));
                }

                if (s.IsTimedOut())
                {
                    Console.WriteLine("Dead connection (" + s.Id + ")");
                    sockets.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!data.Any())
                    continue;

                if (data[0] == TextFrame)
                {
                    ulong size = 0;
                    int payloadIndex = 0;
                    byte[] mask = new byte[4];

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
                    for (int j = 0; j < payload.Length; j++)
                    {
                        payload[j] = (byte)(payload[j] ^ mask[j % 4]);
                    }

                    var jsondata = Encoding.ASCII.GetString(payload, 0, payload.Count());
                    WebSocketMessage wsMessage = null;

                    if (jsondata == "ping")
                    {
                        s.LastMessage = DateTime.Now;
                        Send(s.Stream, "pong");
                        continue;
                    }

                    try
                    {
                        wsMessage = JsonConvert.DeserializeObject<WebSocketMessage>(jsondata);
                    }
                    catch (Exception) { }

                    if (wsMessage != null && wsMessage.HasData())
                    {
                        wsMessage.Connection = s;
                        output.Add(wsMessage);
                    }
                }
            }

            return output;
        }

        public void Send(SslStream stream, string data)
        {
            List<byte> bytes = new List<byte>();

            //text frame
            bytes.Add(129);

            var databytes = Encoding.UTF8.GetBytes(data).ToList();

            if (databytes.Count <= 125)
            {
                bytes.Add((byte)databytes.Count);
                bytes.AddRange(databytes);
            }
            else if (databytes.Count <= 65535)
            {
                bytes.Add(126);
                bytes.Add((byte)((databytes.Count() >> 8) & 255));
                bytes.Add((byte)((databytes.Count()) & 255));
                bytes.AddRange(databytes);
            }
            else
            {
                bytes.Add(127);
                bytes.Add((byte)((databytes.Count() >> 56) & 255));
                bytes.Add((byte)((databytes.Count() >> 48) & 255));
                bytes.Add((byte)((databytes.Count() >> 40) & 255));
                bytes.Add((byte)((databytes.Count() >> 32) & 255));
                bytes.Add((byte)((databytes.Count() >> 24) & 255));
                bytes.Add((byte)((databytes.Count() >> 16) & 255));
                bytes.Add((byte)((databytes.Count() >> 8) & 255));
                bytes.Add((byte)((databytes.Count()) & 255));
                bytes.AddRange(databytes);
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
            List<byte> bytes = new List<byte>();

            //close frame
            bytes.Add(136);

            //length
            bytes.Add(2);

            //code/reason 4001
            bytes.Add(15);
            bytes.Add(161);

            socket.Send(bytes.ToArray());
        }

        private void HandleHttpConnection()
        {
            for (int i = 0; i < _connectingClients.Count; i++)
            {
                var connection = _connectingClients[i];

                string httpRequest = GetHttpRequest(connection);

                if (connection.IsTimeout() || !connection.TcpClient.Connected || httpRequest == null)
                {
                    _connectingClients.RemoveAt(i);
                    i--;
                    continue;
                }

                var httpHeaders = GetHttpHeaders(httpRequest);

                if (httpHeaders.ContainsKey("Sec-WebSocket-Key"))
                {
                    byte[] responseBytes = CreateConnectionResponse(httpHeaders);

                    connection.Stream.Write(responseBytes, 0, responseBytes.Length);

                    var clientConnection = new ClientConnection()
                    {
                        Id = Guid.NewGuid(),
                        Client = connection.TcpClient,
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

        private string GetHttpRequest(ConnectingClient connectingClient)
        {
            string output = string.Empty;

            while (connectingClient.TcpClient.Client.Poll(1000, SelectMode.SelectRead))
            {
                if (!connectingClient.TcpClient.Connected)
                    return null;

                byte[] buffer = new byte[1024];

                int received = 0;
                try
                {
                    received = connectingClient.Stream.Read(buffer, 0, buffer.Length);
                }
                catch (System.IO.IOException) {}

                if (received == 0)
                    break;

                var message = Encoding.ASCII.GetString(buffer, 0, received);
                output += message;
            }

            return output;
        }

        private byte[] CreateConnectionResponse(Dictionary<string, string> headers)
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

        private Dictionary<string, string> GetHttpHeaders(string data)
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

                int split = line.IndexOf(":");
                if (split > -1)
                {
                    var propertyKey = line.Substring(0, split);
                    var propertyValue = line.Substring(split + 1);
                    headers.Add(propertyKey, propertyValue.Trim());
                }
            }

            return headers;
        }

        private byte[] Hash(string input)
        {
            using (var sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        private string GetQueryString(string line, string name)
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