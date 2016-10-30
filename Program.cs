using System;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static IServiceProvider Container { get; private set; }

    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddSingleton<Application, Application>();
        services.AddSingleton<IWebSocketCore, WebSocketCore>();
        services.AddSingleton<IPlayerService, PlayerService>();
        
        services.AddSingleton<IMapComponent, MapComponent>();
        services.AddSingleton<IMovementComponent, MovementComponent>();
        services.AddSingleton<IChatComponent, ChatComponent>();
        services.AddSingleton<IShopComponent, ShopComponent>();
        Container = services.BuildServiceProvider();

        var app = Container.GetService<Application>();
        app.Run();
    }
}
