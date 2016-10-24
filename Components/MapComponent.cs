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

    }
}