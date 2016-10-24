using System;
using System.Net.Sockets;

namespace Rebronx.Server.Models
{
	public class SocketConnection {
        public Guid Id { get; set; }
        public Socket Socket { get; set; }

        public SocketConnection(Guid id, Socket socket)
        {
            Id = id;
            Socket = socket;
        }
    }
}