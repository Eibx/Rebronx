using System.Collections.Generic;
using Rebronx.Server.Models;
using Rebronx.Server.Services.Interfaces;

namespace Rebronx.Server.Components.Map.Services
{
	public class MapService : IMapService
	{
		private Dictionary<int, MapNode> map;

		public MapService()
		{
			

			map = new Dictionary<int, MapNode>
			{
				{ 1, new MapNode { Id = 1, X = 10, Y = 12, Connections = new List<int>{ 2, 6 } } },
				{ 2, new MapNode { Id = 2, X = 22, Y = 8, Connections = new List<int> { 1, 3 } } },
				{ 3, new MapNode { Id = 3, X = 34, Y = 8, Connections = new List<int> { 2, 4, 5 } } },
				{ 4, new MapNode { Id = 4, X = 56, Y = 8, Connections = new List<int> { 3 } } },
				{ 5, new MapNode { Id = 5, X = 34, Y = 22, Connections = new List<int> { 3, 10 } } },

				{ 6, new MapNode { Id = 6, X = 8, Y = 25, Connections = new List<int> { 1, 7 } } },
				{ 7, new MapNode { Id = 7, X = 15, Y = 31, Connections = new List<int> { 6, 8 } } },
				{ 8, new MapNode { Id = 8, X = 20, Y = 36, Connections = new List<int> { 7, 9 } } },
				{ 9, new MapNode { Id = 9, X = 27, Y = 36, Connections = new List<int> { 8, 10 } } },
				{ 10, new MapNode { Id = 10, X = 34, Y = 36, Connections = new List<int> { 9, 5, 11 } } },
				{ 11, new MapNode { Id = 11, X = 42, Y = 36, Connections = new List<int> { 10, 12 } } },
				{ 12, new MapNode { Id = 12, X = 42, Y = 30, Connections = new List<int> { 11 } } }
			};
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