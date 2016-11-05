using System;
using System.Collections.Generic;
using System.Linq;

public class ChatComponent : Component, IChatComponent
{
    private const string Component = "chat";
    private readonly IPlayerService playerService;

    public ChatComponent(IPlayerService playerService)
    {
        this.playerService = playerService;
    }

    public void Run(IList<Message> messages)
    {
        foreach (var message in messages.Where(m => m.Component == Component))
        {
            if (message.Type == "say")
                MessageSay(message);
        }
    }

    public void MessageSay(Message message)
    {
        var inputMessage = GetData<InputChatMessage>(message);

        if (inputMessage != null)
        {
            Console.WriteLine(inputMessage.Message);

            var chatMessage = new SendChatMessage();
            chatMessage.Message = $"{message.Player.Name}: {inputMessage.Message}";

			var position = message?.Player?.Position;
			if (position != null)
            	playerService.SendPosition(message.Player.Position, "lobby", "chat", chatMessage);
        }
    }
}
public class InputChatMessage
{
    public string Message { get; set; }
}

public class SendChatMessage
{
    public string Message { get; set; }
}