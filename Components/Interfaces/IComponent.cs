using System.Collections.Generic;

public interface IComponent
{
	void Run(IList<Message> messages);
}