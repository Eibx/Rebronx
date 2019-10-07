using System;
using System.Collections.Generic;
using System.Linq;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Systems.Combat.Repositories;
using Rebronx.Server.Systems.Combat.Senders;

namespace Rebronx.Server.Systems.Combat
{
    public class CombatSystem : System, ICombatSystem
    {
        private const string Component = "combat";
        private Random random;
        private readonly ICombatSender combatSender;
        private readonly IUserRepository userRepository;
        private readonly ICombatRepository combatRepository;

        public CombatSystem(ICombatSender combatSender, IUserRepository userRepository, ICombatRepository combatRepository)
        {
            random = new Random();
            this.combatSender = combatSender;
            this.userRepository = userRepository;
            this.combatRepository = combatRepository;
        }

        public void Run(IList<Message> messages)
        {
            foreach (var message in messages.Where(m => m.Component == Component))
            {
                if (message.Type == "attack")
                    Attack(message);
            }
        }

        public void Attack(Message message)
        {
            var inputMessage = GetData<InputAttackMessage>(message);

            if (inputMessage != null && message?.Player != null)
            {
                if (message.Player.Id == inputMessage.Victim)
                    return;

                var attacker = message.Player;
                var victim = userRepository.GetPlayerById(inputMessage.Victim);

                if (victim == null)
                    return;

                var attackerStats = combatRepository.GetCombatStats(attacker.Id);
                var victimStats = combatRepository.GetCombatStats(victim.Id);

                var rand = ((float)random.Next(0,100))/100f;
                var hit = (((float)attackerStats.Accuracy/(float)victimStats.Agility)/2.0f)+rand >= 1;

                var damage = (hit) ? 10 : -1;

                combatSender.AttackerReport(attacker, damage);
                combatSender.VictimReport(victim, damage);
            }
        }
    }
    public class InputAttackMessage
    {
        public int Victim { get; set; }
    }
}