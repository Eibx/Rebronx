using System;
using System.Collections.Generic;
using System.Linq;

namespace Rebronx.Server.Systems.Combat.Models
{
    public class Fight
    {
        public Guid Id { get; set; }
        public List<Fighter> AttackingSide { get; set; }
        public List<Fighter> DefendingSide { get; set; }

        public DateTime NextRound { get; set; }

        public Fight(Guid id)
        {
            Id = id;
            AttackingSide = new List<Fighter>();
            DefendingSide = new List<Fighter>();
        }

        public List<int> GetAllPlayerIds()
        {
            var ids = new List<int>();

            ids.AddRange(AttackingSide.Select(x => x.Id));
            ids.AddRange(DefendingSide.Select(x => x.Id));

            return ids;
        }
    }
}