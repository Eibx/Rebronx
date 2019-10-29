namespace Rebronx.Server.Repositories
{
    public interface ITokenRepository
    {
         void SetPlayerToken(Player player, string token);
         string GetToken(Player player);
         void RemovePlayerToken(Player player);
         bool IsTokenAvailable(string token);
    }
}