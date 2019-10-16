namespace Rebronx.Server.Repositories
{
    public interface IUserRepository
    {
        void CreateNewPlayer(string name, string hash, string token);
        Player GetPlayerById(int playerId);
        Player GetPlayerByName(string name);
        Player GetPlayerByLogin(string name, string password);
        Player GetPlayerByToken(string token);
        void RemovePlayer(int playerId);
    }
}