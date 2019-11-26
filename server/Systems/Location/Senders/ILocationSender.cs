using System.Diagnostics;

namespace Rebronx.Server.Systems.Location.Senders
{
    public interface ILocationSender
    {
        void Execute();
        void Update(int node);

    }
}