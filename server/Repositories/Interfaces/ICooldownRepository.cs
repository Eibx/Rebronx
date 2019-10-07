using System;

namespace Rebronx.Server.Repositories.Interfaces
{
    public interface ICooldownRepository
    {
         long? GetCooldown(Player player, string type);
         void SetCooldown(Player player, string type, long miliseconds);

         long? GetAbsoluteCooldown(Player player, string type);
         void SetAbsoluteCooldown(Player player, string type, long unixtime);

    }
}