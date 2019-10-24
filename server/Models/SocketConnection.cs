using System;
using System.Net.Security;
using System.Net.Sockets;

namespace Rebronx.Server.Models
{
    public class ClientConnection {
        public Guid Id { get; set; }
        public TcpClient TcpClient { get; set; }
        public SslStream Stream { get; set; }
        public DateTime LastMessage { get; set; }

        public bool IsTimedOut() {
            return LastMessage.AddSeconds(15) < DateTime.Now;
        }
    }
}