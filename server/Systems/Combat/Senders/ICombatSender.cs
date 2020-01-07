using System.Collections.Generic;
using Rebronx.Server.Systems.Combat.Models;

namespace Rebronx.Server.Systems.Combat.Senders
{
    public interface ICombatSender
    {
        void Report(Fight fight, List<FighterAction> actions);
        void UpdateFight(Fight fight);
        void EndFight(Fight fight);
        void ChangeAttack(Fighter fighter);
        void ChangePosition(Fighter fighter);
    }
}