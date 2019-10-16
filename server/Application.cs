using System.Threading;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Chat;
using Rebronx.Server.Systems.Combat;
using Rebronx.Server.Systems.Inventory;
using Rebronx.Server.Systems.Map;
using Rebronx.Server.Systems.Movement;
using Rebronx.Server.Systems.Store;
using Rebronx.Server.Systems.Command;

public class Application
{
    private readonly IWebSocketCore webSocketCore;
    private readonly IConnectionService connectionService;

    private readonly ICommandSystem commandSystem;
    private readonly IMapSystem mapSystem;
    private readonly IMovementSystem movementSystem;
    private readonly IChatSystem chatSystem;
    private readonly IStoreSystem storeSystem;
    private readonly ICombatSystem combatSystem;
    private readonly IInventorySystem inventorySystem;
    public Application(
        IWebSocketCore webSocketCore,
        IConnectionService connectionService,

        ICommandSystem commandSystem,
        IMapSystem mapSystem,
        IMovementSystem movementSystem,
        IChatSystem chatSystem,
        IStoreSystem storeSystem,
        ICombatSystem combatSystem,
        IInventorySystem inventorySystem)
    {
        this.webSocketCore = webSocketCore;
        this.connectionService = connectionService;

        this.commandSystem = commandSystem;
        this.mapSystem = mapSystem;
        this.movementSystem = movementSystem;
        this.chatSystem = chatSystem;
        this.storeSystem = storeSystem;
        this.combatSystem = combatSystem;
        this.inventorySystem = inventorySystem;
    }

    public void Run()
    {
	    while (true)
        {
            webSocketCore.GetNewConnections();

            connectionService.HandleDeadPlayers();

            var socketMessages = webSocketCore.PollMessages();
            var playerMessages = connectionService.ConvertToMessages(socketMessages);

            commandSystem.Run(playerMessages);
            mapSystem.Run(playerMessages);
            movementSystem.Run(playerMessages);
            chatSystem.Run(playerMessages);
            storeSystem.Run(playerMessages);
            combatSystem.Run(playerMessages);
            inventorySystem.Run(playerMessages);

            Thread.Sleep(1);
        }
    }
}