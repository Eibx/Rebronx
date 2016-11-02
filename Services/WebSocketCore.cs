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
    List<PendingSocket> connectingSockets;
    List<SocketConnection> sockets;
    List<WebSocketMessage> pendingMessages;
    TcpListener server;

    public WebSocketCore()
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 31337);
        server.Start();

        pendingMessages = new List<WebSocketMessage>();
        connectingSockets = new List<PendingSocket>();
        sockets = new List<SocketConnection>();
    }

    public List<SocketConnection> GetNewConnections()
    {
        while (server.Pending())
        {
            var task = server.AcceptSocketAsync();
            task.Wait();
            Socket socket = task.Result;

            Console.WriteLine("A socket connected.");
            connectingSockets.Add(new PendingSocket() { Socket = socket, Connected = DateTime.Now });
        }

        return HandleHttpConnection();
    }

    public List<WebSocketMessage> PollMessages()
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

                if (received == 0)
                    break;

                data.AddRange(buffer.Take(received));
            }

            if (s.IsTimedout()) {
                Console.WriteLine("Dead connection (" + s.Token + ")");
                sockets.RemoveAt(i);
                i--;
                continue;
            }

            if (!data.Any())
                continue;

            s.LastMessage = DateTime.Now;

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
                WebSocketMessage wsMessage = null;

                if (jsondata == "ping")
                    continue;

                try 
                {
                    wsMessage = JsonConvert.DeserializeObject<WebSocketMessage>(jsondata);
                } 
                catch(Exception) { }

                if (wsMessage != null && wsMessage.HasData()) {
                    wsMessage.Socket = s;
                    output.Add(wsMessage);
                }
            }
        }

        return output;
    }

    private List<SocketConnection> HandleHttpConnection()
    {
        var outputSockets = new List<SocketConnection>();

        for (int i = 0; i < connectingSockets.Count; i++)
        {
            var connection = connectingSockets[i];

            string httpRequest = GetHttpRequest(connection);

            if (connection.IsTimeout() || !connection.Socket.Connected || httpRequest == null)
            {
                connectingSockets.RemoveAt(i);
                i--;
                continue;
            }

            var httpHeaders = GetHttpHeaders(httpRequest);

            if (httpHeaders.ContainsKey("Sec-WebSocket-Key"))
            {
                var token = string.Empty;
                httpHeaders.TryGetValue("UserToken", out token);

                byte[] responseBytes = CreateConnectionResponse(httpHeaders);
                var sent = connection.Socket.Send(responseBytes, 0, responseBytes.Length, SocketFlags.None);
                Console.WriteLine(sent + " bytes sent to token " + token);

                var socketConnection = new SocketConnection()
                {
                    Id = Guid.NewGuid(),
                    Socket = connection.Socket,
                    Token = token,
                    LastMessage = DateTime.Now
                };
                sockets.Add(socketConnection);
                outputSockets.Add(socketConnection);

                if (i <= connectingSockets.Count - 1)
                {
                    connectingSockets.RemoveAt(i);
                    i--;
                }
            }
        }

        return outputSockets;
    }

    private string GetHttpRequest(PendingSocket socket)
    {
        string output = string.Empty;

        while (socket.Socket.Poll(1000, SelectMode.SelectRead))
        {
            if (!socket.Socket.Connected)
                return null;

            byte[] buffer = new byte[1024];
            var received = socket.Socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);

            if (received == 0)
                break;

            var message = Encoding.ASCII.GetString(buffer, 0, received);
            output += message;
        }

        return output;
    }

    private byte[] CreateConnectionResponse(Dictionary<string, string> headers)
    {
        var wskey = headers["Sec-WebSocket-Key"];

        var keyhash = Hash(wskey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
        var wskeyResult = Convert.ToBase64String(keyhash);

        var response = "HTTP/1.1 101 Switching Protocols\r\n" +
            "Upgrade: websocket\r\n" +
            "Connection: Upgrade\r\n" +
            "Sec-WebSocket-Accept: " + wskeyResult + "\r\n\r\n";
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
                var token = GetQueryString(line, "TOKEN");
                
                if (token != null)
                    headers.Add("UserToken", token);

                continue;
            }

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

    private string GetQueryString(string line, string name)
    {
        return line.Split(' ')?.Skip(1)?.Take(1).FirstOrDefault()?.Split('?').LastOrDefault()?.Split('&').FirstOrDefault(x => x.StartsWith(name))?.Split('=').LastOrDefault();
    }
}

public class PendingSocket
{
    public Socket Socket { get; set; }
    public DateTime Connected { get; set; }

    public bool IsTimeout() 
    { 
        return (Connected.AddSeconds(5) < DateTime.Now); 
    }
}

public class WebSocketMessage
{
    public SocketConnection Socket { get; set; }
    public string Component { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }

    public bool HasData()
    {
        return !string.IsNullOrEmpty(Component) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Data);
    }
}
