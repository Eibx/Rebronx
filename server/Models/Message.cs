using Rebronx.Server.Enums;

public class Message
{
    public Player Player { get; set; }
    public byte System { get; set; }
    public byte Type { get; set; }
    public string Data { get; set; }
}