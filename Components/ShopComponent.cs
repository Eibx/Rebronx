using System;
using System.Collections.Generic;
using System.Linq;

public class ShopComponent : IShopComponent
{
    private const string Component = "shop";
    private readonly IWebSocketCore webSocketCore;

    public ShopComponent(IWebSocketCore webSocketCore)
    {
        this.webSocketCore = webSocketCore;
    }

    public void Run(IList<Message> messages)
    {
        foreach (var message in messages.Where(m => m.Component == Component))
        {
            if (message.Type == "buy")
                MessageBuy(message);
        }
    }

    public void MessageBuy(Message message)
    {
        Console.WriteLine(message.Data);
    }
}