using Rebronx.Server.Enums;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Inventory.Senders;
using Rebronx.Server.Systems.Location.Senders;

namespace Rebronx.Server.Systems.Join.Senders
{
    public class JoinSender : IJoinSender
    {
        private readonly IMessageService _messageService;
        private readonly ILocationSender _locationSender;
        private readonly IInventorySender _inventorySender;

        public JoinSender(IMessageService messageService, ILocationSender locationSender, IInventorySender inventorySender)
        {
            _messageService = messageService;
            _locationSender = locationSender;
            _inventorySender = inventorySender;
        }

        public void Join(Player player)
        {
            if (player != null)
            {
                var position = player.Node;
                var joinMessage = new JoinResponse();
                joinMessage.Id = player.Id;
                joinMessage.Name = player.Name;
                joinMessage.Node = player.Node;

                //TODO: Send credits - CreditRepository?
                joinMessage.Credits = 0;

                _messageService.Send(player, SystemNames.Join, "join", joinMessage);
                _locationSender.Update(position);
                _inventorySender.SendInventory(player);
            }

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