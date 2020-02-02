using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rebronx.Server.Enums;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Inventory.Senders;
using Rebronx.Server.Systems.Location.Senders;
using Rebronx.Server.Systems.Movement.Senders;

namespace Rebronx.Server.Systems.Join.Senders
{
    public class JoinSender : IJoinSender
    {
        private readonly IMessageService _messageService;
        private readonly ILocationSender _locationSender;
        private readonly IInventorySender _inventorySender;
        private readonly IMovementSender _movementSender;

        private readonly HashSet<Player> _playersToUpdate = new HashSet<Player>();

        public JoinSender(
            IMessageService messageService,
            ILocationSender locationSender,
            IInventorySender inventorySender,
            IMovementSender movementSender)
        {
            _messageService = messageService;
            _locationSender = locationSender;
            _inventorySender = inventorySender;
            _movementSender = movementSender;
        }

        public void Join(Player player)
        {
            if (player != null)
                _playersToUpdate.Add(player);
        }

        public void Execute()
        {
            foreach (var player in _playersToUpdate)
            {
                var joinMessage = new JoinResponse
                {
                    Id = player.Id,
                    Name = player.Name,
                    Node = player.Node,
                    Credits = 0
                };

                _messageService.Send(player, SystemTypes.Join, SystemTypes.JoinTypes.Join, joinMessage);

                _movementSender.StartMove(player);
            }

            _playersToUpdate.Clear();
        }
    }

    public class JoinResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Node { get; set; }
    }
}