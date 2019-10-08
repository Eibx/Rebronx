interface MapConnection {
    id: number,
    cost: number
}

interface MapNode {
    id: number,
    x: number,
    y: number,
    connections: MapConnection[]
}