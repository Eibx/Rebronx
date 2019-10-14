using System.Collections.Generic;

namespace Rebronx.Server.Models
{
    public class MapData
    {
        public List<MapNode> Nodes { get; set; }
    }

    public class MapNode
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public List<MapConnection> Connections { get; set; }
    }

    public class MapConnection {
        public int Id { get; set; }
        public float Cost { get; set; }
    }
}