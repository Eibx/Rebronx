using System;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Combat.Senders
{
    public class CombatSender : ICombatSender
    {
        private IMessageService messageService;
        private ISocketRepository socketRepository;

        public CombatSender(IMessageService messageService, ISocketRepository socketRepository)
        {
            this.messageService = messageService;
            this.socketRepository = socketRepository;
        }

        public void AttackerReport(Player player, int damage)
        {
            var connection = socketRepository.GetConnection(player.Id);
            var combatReport = new SendCombatReport() {
                Damage = damage
            };
            messageService.Send(connection, "combat", "attacker", combatReport);
        }

        public void VictimReport(Player player, int damage)
        {
            var connection = socketRepository.GetConnection(player.Id);
            var combatReport = new SendCombatReport() {
                Damage = damage
            };
            messageService.Send(connection, "combat", "victim", combatReport);
        }
    }

    public class SendCombatReport
    {
        public int Damage { get; set; }
    }
}