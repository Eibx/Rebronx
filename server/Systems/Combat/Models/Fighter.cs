using System.Linq;

namespace Rebronx.Server.Systems.Combat.Models
{
    public class Fighter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool[] Pattern { get; set; }
        public int Position { get; set; }
        public FighterSide Side { get; set; }
        public double Health { get; set; }

        public int PatternSize
        {
            get
            {
                var patternSize = Pattern.Count(x => x == true);
                return patternSize == 0 ? 4 : patternSize;
            }
        }


        public Fighter(int id, string name, int position, FighterSide side)
        {
            Id = id;
            Name = name;
            Pattern = new[] { true, true, true, true };
            Position = position;
            Side = side;
            Health = 100.0d;
        }

        public bool IsAttackingPosition(int position)
        {
            if (position < 0 || position > 3)
                return false;

            return Pattern[position];
        }
    }

    public enum FighterSide
    {
        Attacking,
        Defending
    }
}