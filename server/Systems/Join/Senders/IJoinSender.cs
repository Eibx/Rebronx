using System.Diagnostics;

namespace Rebronx.Server.Systems.Join.Senders
{
    public interface IJoinSender
    {
        void Join(Player player);
    }
}