using System;
using Microsoft.Extensions.DependencyInjection;
using Rebronx.Server.Services;
using Rebronx.Server.Services.Interfaces;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Repositories;
using Rebronx.Server.Components.Chat;
using Rebronx.Server.Components.Map;
using Rebronx.Server.Components.Movement;
using Rebronx.Server.Components.Inventory;
using Rebronx.Server.Components.Join;
using Rebronx.Server.Components.Login;
using Rebronx.Server.Components.Lobby;
using Rebronx.Server.Components.Combat;
using Rebronx.Server.Components.Shop;
using Rebronx.Server.Components.Chat.Senders;
using Rebronx.Server.Components.Inventory.Senders;
using Rebronx.Server.Components.Join.Senders;
using Rebronx.Server.Components.Login.Senders;
using Rebronx.Server.Components.Lobby.Senders;
using Rebronx.Server.Components.Map.Senders;
using Rebronx.Server.Components.Movement.Senders;
using Rebronx.Server.Components.Combat.Senders;
using Rebronx.Server.Components.Map.Services;
using Rebronx.Server.Components.Combat.Repositories;
using Rebronx.Server.Components.Inventory.Repositories;
using Rebronx.Server.Components.Inventory.Services;

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
		services.AddSingleton<IMapComponent, MapComponent>();
		services.AddSingleton<IMovementComponent, MovementComponent>();
		services.AddSingleton<IChatComponent, ChatComponent>();
		services.AddSingleton<IShopComponent, ShopComponent>();
		services.AddSingleton<ICombatComponent, CombatComponent>();
		services.AddSingleton<IInventoryComponent, InventoryComponent>();

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