using System.Diagnostics;

namespace Rebronx.Server.Systems.Join.Senders
{
    public interface IJoinSender
    {
        void Execute();
        void Join(Player player);
    }
}