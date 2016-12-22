using System;
using Microsoft.Extensions.DependencyInjection;
using Rebronx.Server.DataSenders;
using Rebronx.Server.DataSenders.Interfaces;
using Rebronx.Server.Services;
using Rebronx.Server.Services.Interfaces;
using Rebronx.Server.Repositories.Interfaces;
using Rebronx.Server.Repositories;
using StackExchange.Redis;

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
		
		//Components
		services.AddSingleton<IMapComponent, MapComponent>();
		services.AddSingleton<IMovementComponent, MovementComponent>();
		services.AddSingleton<IChatComponent, ChatComponent>();
		services.AddSingleton<IShopComponent, ShopComponent>();

		//Senders
		services.AddSingleton<IChatSender, ChatSender>();
		services.AddSingleton<IInventorySender, InventorySender>();
		services.AddSingleton<IJoinSender, JoinSender>();
		services.AddSingleton<ILoginSender, LoginSender>();
		services.AddSingleton<ILobbySender, LobbySender>();
		services.AddSingleton<IMapSender, MapSender>();
		services.AddSingleton<IMovementSender, MovementSender>();

		//Repositories
		services.AddSingleton<IPlayerRepository, PlayerRepository>();
		services.AddSingleton<ISocketRepository, SocketRepository>();


        Container = services.BuildServiceProvider();

		var app = Container.GetService<Application>();
		app.Run();
	}
}
