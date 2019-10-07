using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Systems.Map.Services
{
    public interface IMapService
    {
        Dictionary<int, MapNode> GetMap();
        MapNode GetNode(int node);
    }
}