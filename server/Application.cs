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
    private readonly IWebSocketCore _webSocketCore;
    private readonly IConnectionService _connectionService;

    private readonly ICommandSystem _commandSystem;
    private readonly IMapSystem _mapSystem;
    private readonly IMovementSystem _movementSystem;
    private readonly IChatSystem _chatSystem;
    private readonly IStoreSystem _storeSystem;
    private readonly ICombatSystem _combatSystem;
    private readonly IInventorySystem _inventorySystem;
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
        _webSocketCore = webSocketCore;
        _connectionService = connectionService;

        _commandSystem = commandSystem;
        _mapSystem = mapSystem;
        _movementSystem = movementSystem;
        _chatSystem = chatSystem;
        _storeSystem = storeSystem;
        _combatSystem = combatSystem;
        _inventorySystem = inventorySystem;
    }

    public void Run()
    {
	    while (true)
        {
            _webSocketCore.GetNewConnections();

            _connectionService.HandleDeadPlayers();

            var socketMessages = _webSocketCore.PollMessages();
            var playerMessages = _connectionService.ConvertToMessages(socketMessages);

            _commandSystem.Run(playerMessages);
            _mapSystem.Run(playerMessages);
            _movementSystem.Run(playerMessages);
            _chatSystem.Run(playerMessages);
            _storeSystem.Run(playerMessages);
            _combatSystem.Run(playerMessages);
            _inventorySystem.Run(playerMessages);

            Thread.Sleep(1);
        }
    }
}