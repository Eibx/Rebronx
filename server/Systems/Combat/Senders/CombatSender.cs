using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Rebronx.Server.Enums;
using Rebronx.Server.Repositories;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Combat.Models;

namespace Rebronx.Server.Systems.Combat.Senders
{
    public class CombatSender : ICombatSender
    {
        private readonly IMessageService _messageService;
        private readonly ISocketRepository _socketRepository;

        public CombatSender(IMessageService messageService, ISocketRepository socketRepository)
        {
            _messageService = messageService;
            _socketRepository = socketRepository;
        }

        public void Report(Fight fight)
        {
            var response = new CombatReportResponse();

            foreach (var fighter in fight.Fighters)
            {
                var action = new CombatReportResponse.FighterAction();
                action.Move = fighter.Position;

                action.Attacks = new List<CombatReportResponse.FighterAttack>();

            }

            foreach (var playerId in fight.GetAllPlayerIds())
            {
                var connection = _socketRepository.GetConnection(playerId);
                _messageService.Send(
                    connection,
                    SystemTypes.Combat,
                    SystemTypes.CombatTypes.Report,
                    response);
            }
        }

        public void UpdateFight(Fight fight)
        {
            var response = new UpdateFightResponse()
            {
                Fighters = fight.Fighters.Select(x => new UpdateFightResponse.SimpleFighter(x)).ToList()
            };

            foreach (var playerId in fight.GetAllPlayerIds())
            {
                var connection = _socketRepository.GetConnection(playerId);
                _messageService.Send(
                    connection,
                    SystemTypes.Combat,
                    SystemTypes.CombatTypes.UpdateFight,
                    response);
            }
        }

        public void EndFight(Fight fight)
        {
            foreach (var playerId in fight.GetAllPlayerIds())
            {
                var connection = _socketRepository.GetConnection(playerId);
                _messageService.Send(
                    connection,
                    SystemTypes.Combat,
                    SystemTypes.CombatTypes.EndFight,
                "");
            }
        }

        public void ChangeAttack(Fighter fighter)
        {
            var connection = _socketRepository.GetConnection(fighter.Id);

            var response = new ChangeAttackResponse()
            {
                Pattern = fighter.Pattern.Take(4).ToArray()
            };

            _messageService.Send(
                connection,
                SystemTypes.Combat,
                SystemTypes.CombatTypes.ChangeAttack,
                response);
        }

        public void ChangePosition(Fighter fighter)
        {
            var connection = _socketRepository.GetConnection(fighter.Id);

            var response = new ChangePositionRequest()
            {
                Position = fighter.Position
            };

            _messageService.Send(
                connection,
                SystemTypes.Combat,
                SystemTypes.CombatTypes.ChangePosition,
                response);
        }
    }

    public class UpdateFightResponse
    {
        public List<SimpleFighter> Fighters { get; set; }

        public class SimpleFighter
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public FighterSide Side { get; set; }

            public SimpleFighter(Fighter fighter)
            {
                Id = fighter.Id;
                Name = fighter.Name;
                Side = fighter.Side;
            }
        }
    }

    public class CombatReportResponse
    {
        public List<FighterAction> Actions { get; set; }
    }

    public class ChangeAttackResponse
    {
        public bool[] Pattern { get; set; }
    }
}