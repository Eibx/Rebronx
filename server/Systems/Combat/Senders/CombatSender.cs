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
    }

    public class CombatReportResponse
    {
        public int Damage { get; set; }
    }
}