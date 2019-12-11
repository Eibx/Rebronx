using System;
using System.Collections;
using System.Collections.Generic;
using Rebronx.Server.Systems.Combat.Models;

namespace Rebronx.Server.Systems.Combat.Repositories
{
    public interface ICombatRepository
    {
        List<Fight> GetAllFights();

        Fight GetFight(Guid id);
        Fighter GetFighter(Fight combat, int playerId);
        Fighter GetFighter(int playerId);

        bool IsPlayerInFight(int id);
        void AddFight(Fight fight);
        void RemoveFights(List<Guid> fightsToDelete);
    }
}