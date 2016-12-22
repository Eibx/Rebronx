using StackExchange.Redis;

namespace Rebronx.Server.Services.Interfaces
{
	public interface IDatabaseService
	{
		IDatabase GetDatabase();
	}
}