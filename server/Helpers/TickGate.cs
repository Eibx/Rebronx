namespace Rebronx.Server.Helpers
{
    public class TickGate
    {
        private readonly uint _ticks;
        private uint _currentTick = 0;

        public TickGate(uint ticks)
        {
            _ticks = ticks;
        }

        public bool Tick()
        {
            _currentTick++;
            if (_currentTick > _ticks)
            {
                _currentTick = 0;
                return true;
            }

            return false;
        }
    }
}