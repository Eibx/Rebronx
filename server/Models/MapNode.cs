using System.Collections.Generic;

namespace Rebronx.Server.Models
{
    public class MapNode
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<int> Connections { get; set; }
    }
}