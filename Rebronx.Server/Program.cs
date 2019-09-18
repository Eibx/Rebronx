using System;
using LightInject;
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
    public static void Main(string[] args)
    {
        var container = new LightInject.ServiceContainer();

        //Services
        container.RegisterSingleton<Application, Application>();
        container.RegisterSingleton<IWebSocketCore, WebSocketCore>();
        container.RegisterSingleton<IConnectionService, ConnectionService>();
        container.RegisterSingleton<IMessageService, MessageService>();
        container.RegisterSingleton<IDatabaseService, DatabaseService>();
        container.RegisterSingleton<IInventoryService, InventoryService>();

        //Components
        container.RegisterSingleton<ICommandSystem, CommandSystem>();
        container.RegisterSingleton<IMapSystem, MapSystem>();
        container.RegisterSingleton<IMovementSystem, MovementSystem>();
        container.RegisterSingleton<IChatSystem, ChatSystem>();
        container.RegisterSingleton<IStoreSystem, StoreSystem>();
        container.RegisterSingleton<ICombatSystem, CombatSystem>();
        container.RegisterSingleton<IInventorySystem, InventorySystem>();

        //Senders
        container.RegisterSingleton<IChatSender, ChatSender>();
        container.RegisterSingleton<IInventorySender, InventorySender>();
        container.RegisterSingleton<IJoinSender, JoinSender>();
        container.RegisterSingleton<ILoginSender, LoginSender>();
        container.RegisterSingleton<ILobbySender, LobbySender>();
        container.RegisterSingleton<IMapSender, MapSender>();
        container.RegisterSingleton<IMovementSender, MovementSender>();
        container.RegisterSingleton<ICombatSender, CombatSender>();
        container.RegisterSingleton<IMapService, MapService>();
        container.RegisterSingleton<IInventorySender, InventorySender>();

        //Repositories
        container.RegisterSingleton<IUserRepository, UserRepository>();
        container.RegisterSingleton<ISocketRepository, SocketRepository>();
        container.RegisterSingleton<IPositionRepository, PositionRepository>();
        container.RegisterSingleton<ITokenRepository, TokenRepository>();
        //container.RegisterSingleton<ICooldownRepository, CooldownRepository>();
        container.RegisterSingleton<ICombatRepository, CombatRepository>();
        container.RegisterSingleton<IInventoryRepository, InventoryRepository>();
        container.RegisterSingleton<IItemRepository, ItemRepository>();

        var app = container.GetInstance<Application>();
        app.Run();
    }
}