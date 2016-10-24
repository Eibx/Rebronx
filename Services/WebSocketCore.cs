using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using Rebronx.Server.Models;
using Newtonsoft.Json;

public class WebSocketCore : IWebSocketCore
{
    private const int TEXT_FRAME = 129;
    private const int BINARY_FRAME = 64;
    List<Socket> connectingSockets;
    List<SocketConnection> sockets;
    List<WebSocketMessage> pendingMessages;
    TcpListener server;

    public WebSocketCore()
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 31337);
        server.Start();

        pendingMessages = new List<WebSocketMessage>();
        connectingSockets = new List<Socket>();
        sockets = new List<SocketConnection>();
    }

    public void HandleNewConnections()
    {
        while (server.Pending())
        {
            var task = server.AcceptSocketAsync();
            task.Wait();
            Socket socket = task.Result;

            Console.WriteLine("A client connected.");
            connectingSockets.Add(socket);
        }

        HandleConnectingSockets();
    }

    public List<WebSocketMessage> GetMessages(string component)
    {
        return pendingMessages.Where(m => m.Component.Equals(component, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public void PollMessages()
    {
        List<WebSocketMessage> output = new List<WebSocketMessage>();

        for (int i = 0; i < sockets.Count; i++)
        {
            var s = sockets[i];

            List<byte> data = new List<byte>();

            while (s.Socket.Poll(1000, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[1024];
                var received = s.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                data.AddRange(buffer.Take(received));
            }

            if (!data.Any())
                continue;

            if (data[0] == TEXT_FRAME)
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
                    payload[j] = (byte)((int)payload[j] ^ (int)mask[j % 4]);
                }

                var jsondata = Encoding.ASCII.GetString(payload, 0, payload.Count());
                var wsMessage = JsonConvert.DeserializeObject<WebSocketMessage>(jsondata);

                if (wsMessage != null && wsMessage.HasData())                
                    output.Add(wsMessage);
            }
        }

        pendingMessages = output;
    }

    private void HandleConnectingSockets()
    {
        for (int i = 0; i < connectingSockets.Count; i++)
        {
            var s = connectingSockets[i];

            string fullmessage = "";

            while (s.Poll(1000, SelectMode.SelectRead))
            {
                if (!s.Connected)
                {
                    connectingSockets.RemoveAt(i);
                    i--;
                }

                byte[] buffer = new byte[1024];
                var received = s.Receive(buffer, 0, buffer.Length, SocketFlags.None);

                if (received == 0)
                    break;

                var message = Encoding.ASCII.GetString(buffer, 0, received);
                fullmessage += message;
            }

            if (fullmessage != string.Empty)
            {
                var headers = GetHeaders(fullmessage);

                if (headers.ContainsKey("Sec-WebSocket-Key"))
                {
                    var wskey = headers["Sec-WebSocket-Key"];

                    var keyhash = Hash(wskey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
                    var wskeyResult = Convert.ToBase64String(keyhash);

                    var response = "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Sec-WebSocket-Accept: " + wskeyResult + "\r\n\r\n";
                    var responseBytes = Encoding.ASCII.GetBytes(response);
                    var sent = s.Send(responseBytes, 0, responseBytes.Length, SocketFlags.None);
                    Console.WriteLine(sent + " bytes sent");

                    sockets.Add(new SocketConnection(Guid.NewGuid(), s));
                    if (i <= connectingSockets.Count - 1)
                    {
                        connectingSockets.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    private Dictionary<string, string> GetHeaders(string data)
    {
        var headers = new Dictionary<string, string>();

        if (!data.StartsWith("GET"))
            return headers;

        var lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith("GET"))
                continue;

            int split = line.IndexOf(":");
            if (split > -1)
            {
                string propertykey = line.Substring(0, split);
                string propertyValue = line.Substring(split + 1);
                headers.Add(propertykey, propertyValue.Trim());
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
}

public class WebSocketMessage
{
    public SocketConnection Socket { get; set; }
    public string Component { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }

    public bool HasData() {
        return !string.IsNullOrEmpty(Component) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Data);
    }
}
