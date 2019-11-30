using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
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
                if (message.Type == SystemTypes.CombatTypes.Attack)
                    ProcessAttackRequest(message);
            }
        }

        private void ProcessAttackRequest(Message message)
        {
            var inputMessage = GetData<AttackRequest>(message);

            if (inputMessage != null && message?.Player != null)
            {
                if (message.Player.Id == inputMessage.Victim)
                    return;

                var attacker = message.Player;
                var victim = _userRepository.GetPlayerById(inputMessage.Victim);

                if (victim == null)
                    return;

                var attackerStats = _combatRepository.GetCombatStats(attacker.Id);
                var victimStats = _combatRepository.GetCombatStats(victim.Id);

                var rand = _random.Next(0,100)/100f;
                var hit = ((float)attackerStats.Accuracy/victimStats.Agility)/2.0f + rand >= 1;
                var damage = (hit) ? 10 : -1;

                _combatSender.AttackerReport(attacker, damage);
                _combatSender.VictimReport(victim, damage);
            }
        }
    }
    public class AttackRequest
    {
        public int Victim { get; set; }
    }
}