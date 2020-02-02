using System.Collections.Generic;
using Rebronx.Server.Systems.Movement;

namespace Rebronx.Server.Repositories
{
    public interface IMovementRepository
    {
        void Add(int playerId, MovementDestination movement);
        MovementDestination Get(int playerId);
        Dictionary<int, MovementDestination> GetAll();
        void Remove(int playerId);
    }
}