/*
The MIT License (MIT)

Copyright (c) 2014 Andrew Hayward

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

https://github.com/andrewhayward/dijkstra/blob/master/LICENSE
*/

export default class Dijkstra {
	constructor(pathData) {
		this.map = pathData;
	}

	findPaths(start, end) {
		var costs = {};
		var open = { '0': [ start ] };
		var predecessors = {};
		var keys;

		var addToOpen = function(cost, vertex) {
			key = "" + cost;
			if (!open[key]) open[key] = [];
			open[key].push(vertex);
		}

		costs[start] = 0;

		while (open) {
			if(!(keys = this.extractKeys(open)).length) break;

			keys.sort(this.sorter);

			var key = keys[0];
			var bucket = open[key];
			var node = bucket.shift();
			var currentCost = parseFloat(key);
			var adjacentNodes = this.map[node] || {};

			if (!bucket.length) delete open[key];

			for (var vertex in adjacentNodes) {
				if (Object.prototype.hasOwnProperty.call(adjacentNodes, vertex)) {
					var cost = adjacentNodes[vertex];
					var totalCost = cost + currentCost;
					var vertexCost = costs[vertex];

					if ((vertexCost === undefined) || (vertexCost > totalCost)) {
						costs[vertex] = totalCost;
						addToOpen(totalCost, vertex);
						predecessors[vertex] = node;
					}
				}
			}
		}

		if (costs[end] === undefined) {
			return null;
		} else {
			return predecessors;
		}

	}

	extractShortest(predecessors, end) {
		var nodes = [];
		var u = end;

		while (u !== undefined) {
			nodes.push(u);
			u = predecessors[u];
		}

		nodes.reverse();
		return nodes;
	}

	findShortestPath(a, b) {
		var nodes = [a, b];
		var start = nodes.shift();
		var end;
		var predecessors;
		var path = [];
		var shortest;

		while (nodes.length) {
			end = nodes.shift();
			predecessors = this.findPaths(start, end);

			if (predecessors) {
				shortest = this.extractShortest(predecessors, end);
				if (nodes.length) {
					path.push.apply(path, shortest.slice(0, -1));
				} else {
					return path.concat(shortest);
				}
			} else {
				return null;
			}

			start = end;
		}
	}
	
	toArray(list, offset) {
		try {
			return Array.prototype.slice.call(list, offset);
		} catch (e) {
			var a = [];
			for (var i = offset || 0, l = list.length; i < l; ++i) {
				a.push(list[i]);
			}
			return a;
		}
	}

	sorter(a, b) {
		return parseFloat(a) - parseFloat(b);
	}

	extractKeys(obj) {
		var keys = [];
		var key;
		for (key in obj) {
			Object.prototype.hasOwnProperty.call(obj,key) && keys.push(key);
		}
		return keys;
	}
}