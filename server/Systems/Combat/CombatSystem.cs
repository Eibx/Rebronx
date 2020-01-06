using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Combat.Models;
using Rebronx.Server.Systems.Combat.Repositories;
using Rebronx.Server.Systems.Combat.Senders;

namespace Rebronx.Server.Systems.Combat
{
    public class CombatSystem : System, ICombatSystem
    {
        private readonly Random _random;
        private readonly ICombatSender _combatSender;
        private readonly IUserRepository _userRepository;
        private readonly ICombatRepository _combatRepository;

        private const int FightRoundTimer = 10;

        public CombatSystem(ICombatSender combatSender, IUserRepository userRepository, ICombatRepository combatRepository)
        {
            _random = new Random();
            _combatSender = combatSender;
            _userRepository = userRepository;
            _combatRepository = combatRepository;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.System == SystemTypes.Combat))
            {
                switch (message.Type)
                {
                    case SystemTypes.CombatTypes.BeginAttack:
                        ProcessAttackRequest(message);
                        break;

                    case SystemTypes.CombatTypes.ChangePosition:
                        ProcessChangePositionRequest(message);
                        break;

                    case SystemTypes.CombatTypes.ChangeAttack:
                        ProcessChangeAttackRequest(message);
                        break;
                }
            }

            ProcessActiveCombats();
        }


        private void ProcessAttackRequest(Message message)
        {
            var inputMessage = GetData<AttackRequest>(message);

            if (inputMessage == null)
                return;

            // Is player trying to attack itself?
            if (message.Player.Id == inputMessage.Victim)
                return;

            var attacker = message.Player;
            var victim = _userRepository.GetPlayerById(inputMessage.Victim);

            if (victim == null)
                return;

            // Make sure the players are at the same position
            if (attacker.Node != victim.Node)
                return;

            // Are the players currently in combat?
            if (_combatRepository.IsPlayerInFight(attacker.Id) ||
                _combatRepository.IsPlayerInFight(victim.Id))
                return;

            // Create new combat
            var fight = new Fight(Guid.NewGuid());
            fight.NextRound = DateTimeOffset.Now.AddSeconds(FightRoundTimer);
            fight.Fighters.Add(new Fighter(attacker.Id, attacker.Name, _random.Next(0, 4), FighterSide.Attacking));
            fight.Fighters.Add(new Fighter(victim.Id, victim.Name,  _random.Next(0, 4), FighterSide.Defending));

            _combatRepository.AddFight(fight);

            _combatSender.UpdateFight(fight);

            foreach (var fighter in fight.Fighters)
            {
                _combatSender.ChangeAttack(fighter);
                _combatSender.ChangePosition(fighter);
            }
        }

        private void ProcessChangePositionRequest(Message message)
        {
            var inputMessage = GetData<ChangePositionRequest>(message);

            if (inputMessage == null)
                return;

            if (inputMessage.Position < 0 || inputMessage.Position > 3)
                return;

            var fighter = _combatRepository.GetFighter(message.Player.Id);

            if (fighter == null)
                return;

            fighter.Position = inputMessage.Position;

            _combatSender.ChangePosition(fighter);
        }

        private void ProcessChangeAttackRequest(Message message)
        {
            var inputMessage = GetData<ChangeAttackRequest>(message);

            if (inputMessage == null)
                return;

            if (inputMessage.Slot < 0 || inputMessage.Slot >= 4)
                return;

            var fighter = _combatRepository.GetFighter(message.Player.Id);

            if (fighter == null)
                return;

            var pattern = fighter.Pattern;

            pattern[inputMessage.Slot] = inputMessage.Active;
            if (pattern.All(x => x == false))
                return;

            fighter.Pattern = pattern;

            _combatSender.ChangeAttack(fighter);
        }

        private void ProcessActiveCombats()
        {
            var fightsToDelete = new List<Guid>();

            foreach (var fight in _combatRepository.GetAllFights())
            {
                if (fight.NextRound > DateTime.Now)
                    continue;

                fight.Round++;
                fight.NextRound = DateTime.Now.AddSeconds(FightRoundTimer);

                var attackers = fight.Fighters.Where(x => x.Side == FighterSide.Attacking).ToList();
                var defenders = fight.Fighters.Where(x => x.Side == FighterSide.Defending).ToList();

                var actions = new List<FighterAction>();
                actions.AddRange(defenders.Select(x => GetFighterAction(x, attackers)));
                actions.AddRange(attackers.Select(x => GetFighterAction(x, defenders)));

                foreach (var attack in actions.SelectMany(x => x.Attacks.SelectMany(s => s.Damages)))
                {
                    var victimFighter = fight.Fighters.FirstOrDefault(x => x.Id == attack.Victim);

                    if (victimFighter == null)
                        continue;

                    victimFighter.Health -= attack.Damage;
                }

                _combatSender.Report(fight);

                if (fight.Round >= 5)
                {
                    _combatSender.EndFight(fight);
                    fightsToDelete.Add(fight.Id);
                }
            }

            _combatRepository.RemoveFights(fightsToDelete);
        }

        private FighterAction GetFighterAction(Fighter fighter, List<Fighter> opponents)
        {
            var action = new FighterAction();
            action.Move = fighter.Position;

            var damagePoints = 10;
            int patternSize = fighter.PatternSize;

            if (patternSize == 0)
                patternSize = 4;

            var damagePercentage = 1d / patternSize;
            var damage = damagePoints * damagePercentage;

            for (int i = 0; i < 4; i++)
            {
                if (!fighter.IsAttackingPosition(i))
                    continue;

                var attackAction = new FighterAttack()
                {
                    Position = i,
                    DamagePercentage = damagePercentage
                };

                var opponentsAtPosition = opponents.Where(x => x.Position == i).ToList();

                foreach (var opponent in opponentsAtPosition)
                {
                    attackAction.Damages.Add(new FighterAttackDamage()
                    {
                        Victim = opponent.Id,
                        Damage = damage
                    });
                }

                action.Attacks.Add(attackAction);
            }

            return action;
        }
    }

    public class ChangePositionRequest
    {
        public int Position { get; set; }
    }

    public class ChangeAttackRequest
    {
        public int Slot { get; set; }
        public bool Active { get; set; }
    }

    public class AttackRequest
    {
        public int Victim { get; set; }
    }
}