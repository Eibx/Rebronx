using Npgsql;

namespace Rebronx.Server.Services
{
    public interface IDatabaseService
    {
        NpgsqlConnection GetConnection();
    }
}
