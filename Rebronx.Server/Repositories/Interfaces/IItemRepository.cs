using Rebronx.Server.Models;

namespace Rebronx.Server.Repositories.Interfaces
{
    public interface IItemRepository
    {
         Item GetItem(int id);
    }
}