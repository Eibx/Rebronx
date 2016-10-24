public class Application
{
    private readonly IWebSocketCore webSocketCore;
    private readonly IMapComponent mapComponent;
    public Application(IWebSocketCore webSocketCore, IMapComponent mapComponent)
    {
        this.webSocketCore = webSocketCore;
    }

    public void Run()
    {
        while (true)
        {
            webSocketCore.HandleNewConnections();
            webSocketCore.PollMessages();

            // Movement (around map and into apartment check privilige)
            // Chat (say, shout, announcements)
            // Send map data to player
            // 
            mapComponent.Run();            
        }
    }
}
