using System;
using Rebronx.Server.Models;

public class LobbyPlayer
{
    public int Id { get; }
    public string Name { get; }
    public int Health { get; }

    public LobbyPlayer(Player player)
    {
        Id = player.Id;
        Name = player.Name;
        Health = player.Health;
    }
}