using System;
using Microsoft.Extensions.DependencyInjection;
using Rebronx.Server.Services;
using Rebronx.Server.Services.Interfaces;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Repositories;
using Rebronx.Server.Systems.Chat;
using Rebronx.Server.Systems.Map;
using Rebronx.Server.Systems.Movement;
using Rebronx.Server.Systems.Inventory;
using Rebronx.Server.Systems.Join;
using Rebronx.Server.Systems.Login;
using Rebronx.Server.Systems.Lobby;
using Rebronx.Server.Systems.Combat;
using Rebronx.Server.Systems.Store;
using Rebronx.Server.Systems.Chat.Senders;
using Rebronx.Server.Systems.Inventory.Senders;
using Rebronx.Server.Systems.Join.Senders;
using Rebronx.Server.Systems.Login.Senders;
using Rebronx.Server.Systems.Lobby.Senders;
using Rebronx.Server.Systems.Map.Senders;
using Rebronx.Server.Systems.Movement.Senders;
using Rebronx.Server.Systems.Combat.Senders;
using Rebronx.Server.Systems.Map.Services;
using Rebronx.Server.Systems.Combat.Repositories;
using Rebronx.Server.Systems.Inventory.Repositories;
using Rebronx.Server.Systems.Inventory.Services;
using Rebronx.Server.Systems.Command;

public class Program
{
    public static IServiceProvider Container { get; private set; }

    public static void Main(string[] args)
    {
        var services = new ServiceCollection();

        //Services
        services.AddSingleton<Application, Application>();
        services.AddSingleton<IWebSocketCore, WebSocketCore>();
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IInventoryService, InventoryService>();

        //Components
        services.AddSingleton<ICommandSystem, CommandSystem>();
        services.AddSingleton<IMapSystem, MapSystem>();
        services.AddSingleton<IMovementSystem, MovementSystem>();
        services.AddSingleton<IChatSystem, ChatSystem>();
        services.AddSingleton<IStoreSystem, StoreSystem>();
        services.AddSingleton<ICombatSystem, CombatSystem>();
        services.AddSingleton<IInventorySystem, InventorySystem>();

        //Senders
        services.AddSingleton<IChatSender, ChatSender>();
        services.AddSingleton<IInventorySender, InventorySender>();
        services.AddSingleton<IJoinSender, JoinSender>();
        services.AddSingleton<ILoginSender, LoginSender>();
        services.AddSingleton<ILobbySender, LobbySender>();
        services.AddSingleton<IMapSender, MapSender>();
        services.AddSingleton<IMovementSender, MovementSender>();
        services.AddSingleton<ICombatSender, CombatSender>();
        services.AddSingleton<IMapService, MapService>();
        services.AddSingleton<IInventorySender, InventorySender>();

        //Repositories
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ISocketRepository, SocketRepository>();
        services.AddSingleton<IPositionRepository, PositionRepository>();
        services.AddSingleton<ITokenRepository, TokenRepository>();
        //services.AddSingleton<ICooldownRepository, CooldownRepository>();
        services.AddSingleton<ICombatRepository, CombatRepository>();
        services.AddSingleton<IInventoryRepository, InventoryRepository>();
        services.AddSingleton<IItemRepository, ItemRepository>();


        Container = services.BuildServiceProvider();

        var app = Container.GetService<Application>();
        app.Run();
    }
}