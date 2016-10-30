using System;

public class ShopComponent : IShopComponent
{
	private const string Component = "shop";
	private readonly IWebSocketCore webSocketCore;

	public ShopComponent(IWebSocketCore webSocketCore)
	{
		this.webSocketCore = webSocketCore;
	}

    public void Run()
    {
		
    }
}