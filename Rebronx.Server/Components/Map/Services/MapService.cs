using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rebronx.Server.Models;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Map.Services
{
    public class MapService : IMapService
    {
        private Dictionary<int, MapNode> map;

        public MapService()
        {
            var mapFile = File.ReadAllText("../Rebronx.Data/map.json");

            dynamic mapJson = JsonConvert.DeserializeObject(mapFile);

            map = new Dictionary<int, MapNode>();
            foreach(var node in mapJson.map)
            {
                map.Add((int)node.id, new MapNode() {
                    Id = (int)node.id,
                    X = (int)node.posX,
                    Y = (int)node.posY,
                    Connections = node.connections.ToObject<List<int>>()
                });
            }
        }

        public Dictionary<int, MapNode> GetMap()
        {
            return this.map;
        }
        public MapNode GetNode(int node)
        {
            if (!this.map.ContainsKey(node))
            {
                return null;
            }

            return this.map[node];
        }
    }
}