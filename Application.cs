public class Application
{
    private readonly IWebSocketCore webSocketCore;
    private readonly IPlayerService playerService;

    private readonly IMapComponent mapComponent;
    private readonly IMovementComponent movementComponent;
    private readonly IChatComponent chatComponent;
    private readonly IShopComponent shopComponent;
    public Application(
        IWebSocketCore webSocketCore,
        IPlayerService playerService, 
        IMapComponent mapComponent,
        IMovementComponent movementComponent,
        IChatComponent chatComponent,
        IShopComponent shopComponent)
    {
        this.webSocketCore = webSocketCore;
        this.playerService = playerService;

        this.mapComponent = mapComponent;
        this.movementComponent = movementComponent;
        this.chatComponent = chatComponent;
        this.shopComponent = shopComponent;
    }

    public void Run()
    {
        while (true)
        {
            var connections = webSocketCore.GetNewConnections();
            playerService.HandleNewPlayers(connections);
            var socketMessages = webSocketCore.PollMessages();
            var playerMessages = playerService.ConvertToPlayerMessages(socketMessages);

            mapComponent.Run(playerMessages);
            movementComponent.Run(playerMessages);
            chatComponent.Run(playerMessages);
            shopComponent.Run(playerMessages);
            
        }
    }
}
