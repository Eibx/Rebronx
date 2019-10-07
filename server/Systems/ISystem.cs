using System.Collections.Generic;

namespace Rebronx.Server.Systems
{
    public interface ISystem
    {
        void Run(IList<Message> messages);
    }
}
