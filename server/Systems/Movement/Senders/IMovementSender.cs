using System.Collections.Generic;

namespace Rebronx.Server.Systems.Movement.Senders
{
    public interface IMovementSender
    {
        void StartMove(Player player, List<int> nodes, long moveTime);
        void SetPosition(Player player, int newNode);
    }
}