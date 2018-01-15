using System.Threading;
using Rebronx.Server.Components.Chat;
using Rebronx.Server.Components.Combat;
using Rebronx.Server.Components.Inventory;
using Rebronx.Server.Components.Map;
using Rebronx.Server.Components.Movement;
using Rebronx.Server.Components.Store;
using Rebronx.Server.Components.Command;
using Rebronx.Server.Services.Interfaces;

public class Application
{
	private readonly IWebSocketCore webSocketCore;
	private readonly IConnectionService connectionService;

	private readonly ICommandComponent commandComponent;
	private readonly IMapComponent mapComponent;
	private readonly IMovementComponent movementComponent;
	private readonly IChatComponent chatComponent;
	private readonly IStoreComponent storeComponent;
	private readonly ICombatComponent combatComponent;
	private readonly IInventoryComponent inventoryComponent;
	public Application(
		IWebSocketCore webSocketCore,
		IConnectionService connectionService,

		ICommandComponent commandComponent,
		IMapComponent mapComponent,
		IMovementComponent movementComponent,
		IChatComponent chatComponent,
		IStoreComponent storeComponent,
		ICombatComponent combatComponent,
		IInventoryComponent inventoryComponent)
	{
		this.webSocketCore = webSocketCore;
		this.connectionService = connectionService;

		this.commandComponent = commandComponent;
		this.mapComponent = mapComponent;
		this.movementComponent = movementComponent;
		this.chatComponent = chatComponent;
		this.storeComponent = storeComponent;
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

			commandComponent.Run(playerMessages);
			mapComponent.Run(playerMessages);
			movementComponent.Run(playerMessages);
			chatComponent.Run(playerMessages);
			storeComponent.Run(playerMessages);
			combatComponent.Run(playerMessages);
			inventoryComponent.Run(playerMessages);			

			Thread.Sleep(1);
		}
	}
}