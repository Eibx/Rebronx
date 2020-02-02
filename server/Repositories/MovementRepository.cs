using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Systems.Movement;

namespace Rebronx.Server.Repositories
{
    public class MovementRepository : IMovementRepository
    {
        private readonly Dictionary<int, MovementDestination> _movements = new Dictionary<int, MovementDestination>();


        public void Add(int playerId, MovementDestination movement)
        {
            _movements[playerId] = movement;
        }

        public MovementDestination Get(int playerId)
        {
            if (!_movements.ContainsKey(playerId))
                return null;

            return _movements[playerId];
        }

        public Dictionary<int, MovementDestination> GetAll()
        {
            return _movements;
        }

        public void Remove(int playerId)
        {
            if (_movements.ContainsKey(playerId))
                _movements.Remove(playerId);
        }
    }
}