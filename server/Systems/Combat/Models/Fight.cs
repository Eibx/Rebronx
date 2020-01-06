using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebronx.Server.Systems.Combat.Models
{
    public class Fight
    {
        public Guid Id { get; set; }
        public int Round { get; set; }
        public List<Fighter> Fighters { get; set; }

        public DateTimeOffset NextRound { get; set; }

        public Fight(Guid id)
        {
            Id = id;
            Fighters = new List<Fighter>();
        }

        public List<int> GetAllPlayerIds()
        {
            var ids = new List<int>();
            ids.AddRange(Fighters.Select(x => x.Id));
            return ids;
        }
    }
}