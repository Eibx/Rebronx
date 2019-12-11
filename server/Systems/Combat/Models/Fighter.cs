namespace Rebronx.Server.Systems.Combat.Models
{
    public class Fighter
    {
        public int Id { get; set; }
        public bool[] Pattern { get; set; }

        public int Position { get; set; }

        public Fighter(int id, int position)
        {
            Id = id;
            Pattern = new[] { true, true, true, true };
            Position = position;
        }
    }
}