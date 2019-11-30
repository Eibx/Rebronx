using System.Collections.Generic;
using Rebronx.Server.Models;

namespace Rebronx.Server.Services
{
    public interface IMapService
    {
        Dictionary<int, MapNode> GetMap();
        MapNode GetNode(int node);
    }
}