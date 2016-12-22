using System.Threading;
using Rebronx.Server.Services.Interfaces;

public class Application
{
	private readonly IWebSocketCore webSocketCore;
	private readonly IConnectionService connectionService;

	private readonly IMapComponent mapComponent;
	private readonly IMovementComponent movementComponent;
	private readonly IChatComponent chatComponent;
	private readonly IShopComponent shopComponent;
	public Application(
		IWebSocketCore webSocketCore,
		IConnectionService connectionService,

		IMapComponent mapComponent,
		IMovementComponent movementComponent,
		IChatComponent chatComponent,
		IShopComponent shopComponent)
	{
		this.webSocketCore = webSocketCore;
		this.connectionService = connectionService;

		this.mapComponent = mapComponent;
		this.movementComponent = movementComponent;
		this.chatComponent = chatComponent;
		this.shopComponent = shopComponent;
	}

	public void Run()
	{
		while (true)
		{
			webSocketCore.GetNewConnections();

			connectionService.HandleDeadPlayers();
			
			var socketMessages = webSocketCore.PollMessages();
			var playerMessages = connectionService.ConvertToMessages(socketMessages);

			mapComponent.Run(playerMessages);
			movementComponent.Run(playerMessages);
			chatComponent.Run(playerMessages);
			shopComponent.Run(playerMessages);

			Thread.Sleep(1);
		}
	}
}
