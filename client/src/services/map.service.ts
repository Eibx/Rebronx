class MapService {
    private map = require('../../../data/map.json');
    private pathfinder: any = null;

    constructor() {
        let path = require('ngraph.path');
        let createGraph = require('ngraph.graph');

        var graph = createGraph();

        for (let i = 0; i < this.map.nodes.length; i++) {
            const node = this.map.nodes[i];
            graph.addNode(node.id, { x: node.x, y: node.y });

            for (let n = 0; n < node.connections.length; n++) {
                const connection = node.connections[n];
                graph.addLink(node.id, connection);
            }
        }

        this.pathfinder = path.aStar(graph);
    }

    public getAllLocations() {
        let output = [];
        for (let i = 0; i < this.map.nodes.length; i++) {
            const element = this.map.nodes[i];
            if (!element.step) {
                output.push(element);
            }
        }
        return output;
    }

    public getAllSteps() {
        let output = [];
        for (let i = 0; i < this.map.nodes.length; i++) {
            const element = this.map.nodes[i];
            if (element.step) {
                output.push(element);
            }
        }
        return output;
    }

    public getAllNodes() {
        return this.map.nodes;
    }

    public getNode(id: number): MapNode | null {
        for (let i = 0; i < this.map.nodes.length; i++) {
            if (this.map.nodes[i].id == id) {
                return this.map.nodes[i];
            }
        }

        return null;
    }

    public getCloseLocation(x: number, y: number): MapNode | null {
        let closestDist: number | null = null;
        let closestNode: MapNode | null = null;
        let locations = this.getAllLocations();

        for (let i = 0; i < locations.length; i++) {
            const node = locations[i];
            const a = Math.pow(node.x - x, 2);
            const b = Math.pow(node.y - y, 2);
            const dist = Math.sqrt(a+b);

            if (dist < 0.75 && (closestDist === null || dist < closestDist)) {
                closestDist = dist;
                closestNode = node;
            }
        }

        return closestNode;
    }

    public getShortestPath(start: number, end: number): number[] {
        let output: number[] = [];
        let paths = this.pathfinder.find(start, end);

        for (let i = 0; i < paths.length; i++) {
            output.push(paths[i].id);
        }

        return output.reverse();
    }
}

export default new MapService();