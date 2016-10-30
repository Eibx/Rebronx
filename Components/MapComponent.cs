public class MapComponent : IMapComponent
{
    private const string Component = "map";
    private readonly IWebSocketCore webSocketCore;

    public MapComponent(IWebSocketCore webSocketCore)
    {
        this.webSocketCore = webSocketCore;
    }

    public void Run()
    {
        var messages = webSocketCore.GetMessages(Component);

        foreach (var message in messages)
        {
            if (message.Type == "data")
                MessageData();
        }
    }

    public void MessageData()
    {
        
    }
}