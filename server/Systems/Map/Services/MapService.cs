using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rebronx.Server.Models;

namespace Rebronx.Server.Systems.Map.Services
{
    public class MapService : IMapService
    {
        private Dictionary<int, MapNode> map;

        public MapService()
        {
            var mapFile = File.ReadAllText("../data/map.json");

            MapData mapData = JsonConvert.DeserializeObject<MapData>(mapFile);

            map = new Dictionary<int, MapNode>();
            foreach(var node in mapData.Nodes)
            {
                if (node == null)
                    continue;

                map.Add(node.Id, node);
            }
        }

        public Dictionary<int, MapNode> GetMap()
        {
            return map;
        }

        public MapNode GetNode(int node)
        {
            if (!map.ContainsKey(node))
            {
                return null;
            }

            return map[node];
        }
    }
}