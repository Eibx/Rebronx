using System.Threading;
using Rebronx.Server.Components.Chat;
using Rebronx.Server.Components.Combat;
using Rebronx.Server.Components.Inventory;
using Rebronx.Server.Components.Map;
using Rebronx.Server.Components.Movement;
using Rebronx.Server.Components.Shop;
using Rebronx.Server.Services.Interfaces;

public class Application
{
	private readonly IWebSocketCore webSocketCore;
	private readonly IConnectionService connectionService;

	private readonly IMapComponent mapComponent;
	private readonly IMovementComponent movementComponent;
	private readonly IChatComponent chatComponent;
	private readonly IShopComponent shopComponent;
	private readonly ICombatComponent combatComponent;
	private readonly IInventoryComponent inventoryComponent;
	public Application(
		IWebSocketCore webSocketCore,
		IConnectionService connectionService,

		IMapComponent mapComponent,
		IMovementComponent movementComponent,
		IChatComponent chatComponent,
		IShopComponent shopComponent,
		ICombatComponent combatComponent,
		IInventoryComponent inventoryComponent)
	{
		this.webSocketCore = webSocketCore;
		this.connectionService = connectionService;

		this.mapComponent = mapComponent;
		this.movementComponent = movementComponent;
		this.chatComponent = chatComponent;
		this.shopComponent = shopComponent;
		this.combatComponent = combatComponent;
		this.inventoryComponent = inventoryComponent;
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
			combatComponent.Run(playerMessages);
			inventoryComponent.Run(playerMessages);			

			Thread.Sleep(1);
		}
	}
}