namespace Rebronx.Server.Models
{
    public class UnauthorizedMessage : Message
    {
        public ClientConnection Connection { get; set; }
    }
}