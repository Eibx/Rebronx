import {dataService} from "@/services/data.service";

class MapService {
    private map = require('../../../data/map.json');
    private pathfinder: any = null;

    private startTravelTime: number = 0;
    private endTravelTime: number = 0;

    private activePath: MapConnection[] = [];
    private totalCost: number = 0;

    constructor() {
        let path = require('ngraph.path');
        let createGraph = require('ngraph.graph');

        const graph = createGraph();

        for (let i = 0; i < this.map.nodes.length; i++) {
            const node = this.map.nodes[i];
            graph.addNode(node.id, { x: node.x, y: node.y });

            for (let n = 0; n < node.connections.length; n++) {
                const connectionId = node.connections[n].id;
                graph.addLink(node.id, connectionId);
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

    public setActivePath(paths: number[], moveTime: number) {
        this.startTravelTime = new Date().getTime();
        this.endTravelTime = this.startTravelTime + moveTime;

        console.log("end travel time", this.endTravelTime, moveTime);

        this.activePath = [];
        this.totalCost = 0;

        this.activePath.push({ id: paths[0], cost: 0 });

        for (let i = 1; i < paths.length; i++) {
            const previous = this.getNode(paths[i-1]);
            if (previous === null)
                continue;

            let connection = previous.connections.filter((x) => x.id == paths[i])[0];
            this.activePath.push(connection);
        }

        for (let i = 1; i < this.activePath.length; i++) {
            this.totalCost += this.activePath[i].cost;
        }
    }

    public getActiveStep(): any {
        if (this.activePath.length === 0)
            return null;

        const totalTravelTime = this.endTravelTime - this.startTravelTime;
        const currentTravelTime = new Date().getTime() - this.startTravelTime;

        if (currentTravelTime > totalTravelTime)
            return null;

        const percentageTime = currentTravelTime / totalTravelTime;
        const percentageCost = this.totalCost * percentageTime;

        let currentCost = 0;
        let previousCost = 0;
        let index = 0;
        let percentage = 0;

        for (let i = 1; i < this.activePath.length; i++) {
            const path = this.activePath[i];

            index = i;

            previousCost = currentCost;
            currentCost += path.cost;

            if (currentCost < percentageCost) {
                continue;
            }

            percentage = (percentageCost - previousCost) / (currentCost - previousCost);
            break;
        }

        return {
            travel: percentage,
            index: index
        }
    }

    public startTravel(path: number[]): void {
        dataService.send("movement", "move", { nodes: path });
    }
}

export const mapService = new MapService();