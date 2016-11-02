using System;
using System.Net.Sockets;

namespace Rebronx.Server.Models
{
	public class SocketConnection {
        public Guid Id { get; set; }
        public Socket Socket { get; set; }
        public string Token { get; set; }

        public DateTime LastMessage { get; set; }

        public bool IsTimedout() {
            return LastMessage.AddSeconds(15) < DateTime.Now;
        }

        public SocketConnection() 
        {
        }
    }
}