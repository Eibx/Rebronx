import DataService from './data.service'
import MapService from './map.service'
import RenderService from './render.service'
import PlayerStore from '../stores/player.store'
import WorldStore from '../stores/world.store'

class PlayerService {
    public setup() {
        DataService.subscribe('join', (type: string, data: any) => {
            PlayerStore.isAuthenticated = true;
            PlayerStore.name = data.name;
            PlayerStore.bits = data.credits;
            WorldStore.currentNode = data.node;
        });

        DataService.subscribe('player', (type: string, data: any) => {
            if (type === 'position') {
                WorldStore.currentNode = data.node;
            }

            if (type === 'movement') {
                MapService.setActivePath(data.nodes, data.movetime);
                RenderService.setActivePath(data.nodes);
            }
        });
    }

    constructor() {
    }
}

export default new PlayerService();
