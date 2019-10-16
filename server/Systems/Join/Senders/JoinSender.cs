using System;
using System.Linq;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Lobby;
using Rebronx.Server.Systems.Inventory.Senders;
using Rebronx.Server.Systems.Lobby.Senders;

namespace Rebronx.Server.Systems.Join.Senders
{
    public class JoinSender : IJoinSender
    {
        private readonly IMessageService messageService;
        private readonly ILobbySender lobbySender;
        private readonly IInventorySender inventorySender;

        public JoinSender(IMessageService messageService, ILobbySender lobbySender, IInventorySender inventorySender)
        {
            this.messageService = messageService;
            this.lobbySender = lobbySender;
            this.inventorySender = inventorySender;
        }

        public void Join(Player player)
        {
            if (player != null)
            {
                var position = player.Node;
                var joinMessage = new SendJoinMessage();
                joinMessage.Id = player.Id;
                joinMessage.Name = player.Name;
                joinMessage.Node = player.Node;

                //TODO: Send credits - CreditRepository?
                joinMessage.Credits = 0;

                messageService.Send(player, "join", "join", joinMessage);
                lobbySender.Update(position);
                inventorySender.SendInventory(player);
            }

        }
    }

    public class SendJoinMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Node { get; set; }
    }
}