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
        private const string Component = "combat";
        private Random _random;
        private readonly ICombatSender _combatSender;
        private readonly IUserRepository _userRepository;
        private readonly ICombatRepository _combatRepository;

        private const int fightRoundTimer = 6;

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
            var activeCombat = new Fight(Guid.NewGuid());
            activeCombat.NextRound = DateTime.Now.AddSeconds(fightRoundTimer*2);
            activeCombat.AttackingSide.Add(new Fighter(attacker.Id, _random.Next(0, 4)));
            activeCombat.DefendingSide.Add(new Fighter(victim.Id,  _random.Next(0, 4)));

            _combatRepository.AddFight(activeCombat);
        }

        private void ProcessChangePositionRequest(Message message)
        {
            var inputMessage = GetData<ChangePositionRequest>(message);

            if (inputMessage == null)
                return;

            if (inputMessage.Position < 0 || inputMessage.Position > 3)
                return;

            var fighter = _combatRepository.GetFighter(message.Player.Id);

            if (fighter != null)
                fighter.Position = inputMessage.Position;
        }

        private void ProcessChangeAttackRequest(Message message)
        {
            var inputMessage = GetData<ChangeAttackRequest>(message);

            if (inputMessage == null)
                return;

            if (inputMessage.Pattern.Length != 4)
                return;

            if (inputMessage.Pattern.All(x => x == false))
                return;

            var fighter = _combatRepository.GetFighter(message.Player.Id);

            if (fighter != null)
                fighter.Pattern = inputMessage.Pattern;
        }

        private void ProcessActiveCombats()
        {
            var fightsToDelete = new List<Guid>();

            foreach (var fight in _combatRepository.GetAllFights())
            {
                if (fight.NextRound > DateTime.Now)
                    continue;

                var roundReportEntries = new List<RoundReportEntry>();

                fight.NextRound = DateTime.Now.AddSeconds(fightRoundTimer);

                foreach (var fighter in fight.DefendingSide)
                {
                    var reports = FightOpponents(fighter, fight.AttackingSide);
                    roundReportEntries.AddRange(reports);
                }

                foreach (var fighter in fight.AttackingSide)
                {
                    var reports = FightOpponents(fighter, fight.DefendingSide);
                    roundReportEntries.AddRange(reports);
                }
            }

            _combatRepository.RemoveFights(fightsToDelete);
        }

        private List<RoundReportEntry> FightOpponents(Fighter fighter, List<Fighter> opponents)
        {
            var damagePoints = 10;

            int patternSize = fighter.Pattern.Count(x => x == true);

            if (patternSize == 0)
                patternSize = 4;

            var damagePercentage = 1d / patternSize;
            var damage = damagePoints / damagePercentage;

            var reportEntries = new List<RoundReportEntry>();
            foreach (var opponent in opponents)
            {
                if (opponent.Position < 0 || opponent.Position > 3)
                    continue;

                if (fighter.Pattern[opponent.Position] == false)
                    continue;

                reportEntries.Add(new RoundReportEntry()
                {
                    Attacker = fighter.Id,
                    Victim = opponent.Id,
                    Damage = damage,
                    DamagePercentage = damagePercentage
                });
            }

            return reportEntries;
        }
    }

    public class RoundReportEntry
    {
        public int Attacker { get; set; }
        public int Victim { get; set; }
        public double Damage { get; set; }
        public double DamagePercentage { get; set; }
    }

    public class ChangePositionRequest
    {
        public int Position { get; set; }
    }

    public class ChangeAttackRequest
    {
        public bool[] Pattern { get; set; }
    }

    public class AttackRequest
    {
        public int Victim { get; set; }
    }
}