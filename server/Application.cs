using System;
using System.Diagnostics;
using System.Threading;
using Rebronx.Server.Services;
using Rebronx.Server.Systems.Chat;
using Rebronx.Server.Systems.Chat.Senders;
using Rebronx.Server.Systems.Combat;
using Rebronx.Server.Systems.Inventory;
using Rebronx.Server.Systems.Map;
using Rebronx.Server.Systems.Movement;
using Rebronx.Server.Systems.Store;
using Rebronx.Server.Systems.Command;
using Rebronx.Server.Systems.Location.Senders;
using Rebronx.Server.Systems.Login;

public class Application
{
    private readonly IWebSocketCore _webSocketCore;
    private readonly IConnectionService _connectionService;

    private readonly ILoginSystem _loginSystem;
    private readonly ICommandSystem _commandSystem;
    private readonly IMapSystem _mapSystem;
    private readonly IMovementSystem _movementSystem;
    private readonly IChatSystem _chatSystem;
    private readonly IStoreSystem _storeSystem;
    private readonly ICombatSystem _combatSystem;
    private readonly IInventorySystem _inventorySystem;

    private readonly ILocationSender _locationSender;
    private readonly IChatSender _chatSender;


    public Application(
        IWebSocketCore webSocketCore,
        IConnectionService connectionService,

        ILoginSystem loginSystem,
        ICommandSystem commandSystem,
        IMapSystem mapSystem,
        IMovementSystem movementSystem,
        IChatSystem chatSystem,
        IStoreSystem storeSystem,
        ICombatSystem combatSystem,
        IInventorySystem inventorySystem,

        ILocationSender locationSender,
        IChatSender chatSender)
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
        _loginSystem = loginSystem;

        _locationSender = locationSender;
        _chatSender = chatSender;
    }

    public void Run()
    {
        var stopwatch = new Stopwatch();
	    while (true)
        {
            try
            {
                _webSocketCore.GetNewConnections();

                _connectionService.HandleDeadPlayers();

                var socketMessages = _webSocketCore.PollMessages();
                var playerMessages = _connectionService.ConvertToMessages(socketMessages);

                _loginSystem.Run(playerMessages);
                _commandSystem.Run(playerMessages);
                _mapSystem.Run(playerMessages);
                _movementSystem.Run(playerMessages);
                _chatSystem.Run(playerMessages);
                _storeSystem.Run(playerMessages);
                _combatSystem.Run(playerMessages);
                _inventorySystem.Run(playerMessages);

                _locationSender.Execute();
                _chatSender.Execute();

                Thread.Sleep(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("===== EXCEPTION =====");
                Console.WriteLine(e.ToString());
                Console.WriteLine("=====================");
            }
        }
    }
}