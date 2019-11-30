using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rebronx.Server.Models;

namespace Rebronx.Server.Services
{
    public class MapService : IMapService
    {
        private Dictionary<int, MapNode> _map;

        public MapService()
        {
            var mapFile = File.ReadAllText("../data/map.json");

            MapData mapData = JsonConvert.DeserializeObject<MapData>(mapFile);

            _map = new Dictionary<int, MapNode>();
            foreach(var node in mapData.Nodes)
            {
                if (node == null)
                    continue;

                _map.Add(node.Id, node);
            }
        }

        public Dictionary<int, MapNode> GetMap()
        {
            return _map;
        }

        public MapNode GetNode(int node)
        {
            if (!_map.ContainsKey(node))
            {
                return null;
            }

            return _map[node];
        }
    }
}