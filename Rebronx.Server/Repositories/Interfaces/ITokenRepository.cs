namespace Rebronx.Server.Repositories.Interfaces
{
    public interface ITokenRepository
    {
         void SetPlayerToken(Player player, string token);
		 string GetToken(Player player);
		 void RemovePlayerToken(Player player);
    }
}