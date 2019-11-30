using System;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;

namespace Rebronx.Server.Systems.Combat.Senders
{
    public class CombatSender : ICombatSender
    {
        private IMessageService _messageService;
        private ISocketRepository _socketRepository;

        public CombatSender(IMessageService messageService, ISocketRepository socketRepository)
        {
            _messageService = messageService;
            _socketRepository = socketRepository;
        }

        public void AttackerReport(Player player, int damage)
        {
            var connection = _socketRepository.GetConnection(player.Id);
            var combatReport = new CombatReportResponse() {
                Damage = damage
            };
            _messageService.Send(connection, SystemTypes.Combat, SystemTypes.CombatTypes.AttckerReport, combatReport);
        }

        public void VictimReport(Player player, int damage)
        {
            var connection = _socketRepository.GetConnection(player.Id);
            var combatReport = new CombatReportResponse() {
                Damage = damage
            };
            _messageService.Send(connection, SystemTypes.Combat, SystemTypes.CombatTypes.VictimReport, combatReport);
        }
    }

    public class CombatReportResponse
    {
        public int Damage { get; set; }
    }
}