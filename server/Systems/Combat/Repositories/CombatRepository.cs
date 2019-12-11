using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Models;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Combat.Models;

namespace Rebronx.Server.Systems.Combat.Repositories
{
    public class CombatRepository : ICombatRepository
    {
        private readonly Dictionary<int, Fight> _fighters = new Dictionary<int, Fight>();
        private readonly Dictionary<Guid, Fight> _fights = new Dictionary<Guid, Fight>();

        public CombatRepository()
        {

        }

        public List<Fight> GetAllFights()
        {
            return _fights.Values.ToList();
        }

        public Fight GetFight(Guid id)
        {
            return _fights.ContainsKey(id) ? _fights[id] : null;
        }

        public Fighter GetFighter(Fight combat, int playerId)
        {
            var fighter = combat.AttackingSide.FirstOrDefault(x => x.Id == playerId);

            if (fighter == null)
                fighter = combat.DefendingSide.FirstOrDefault(x => x.Id == playerId);

            return fighter;
        }

        public Fighter GetFighter(int playerId)
        {
            if (_fighters.ContainsKey(playerId))
            {
                // Cleanup if fight is null, but player still exists as a fighter
                if (_fighters[playerId] == null)
                {
                    _fighters.Remove(playerId);
                    return null;
                }

                return GetFighter(_fighters[playerId], playerId);
            }

            return null;
        }

        public bool IsPlayerInFight(int id)
        {
            return _fighters.ContainsKey(id);
        }

        public void AddFight(Fight fight)
        {
            _fights.Add(fight.Id, fight);

            foreach (var player in fight.AttackingSide)
            {
                _fighters[player.Id] = fight;
            }

            foreach (var player in fight.DefendingSide)
            {
                _fighters[player.Id] = fight;
            }
        }

        public void RemoveFights(List<Guid> fightsToDelete)
        {
            foreach (var id in fightsToDelete)
            {
                if (!_fights.ContainsKey(id))
                    continue;

                var playerIds = _fights[id].GetAllPlayerIds();

                foreach (var playerId in playerIds)
                {
                    if (_fighters.ContainsKey(playerId))
                        _fighters.Remove(playerId);
                }
            }
        }
    }
}