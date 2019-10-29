import {dataService} from './data.service'
import {mapService} from './map.service'
import {renderService} from './render.service'
import {playerStore} from '../stores/player.store'
import {worldStore} from '../stores/world.store'

class PlayerService {
    public setup() {
        dataService.subscribe('join', (type: string, data: any) => {
            playerStore.isAuthenticated = true;
            playerStore.name = data.name;
            playerStore.bits = data.credits;
            worldStore.currentNode = data.node;
        });

        dataService.subscribe('player', (type: string, data: any) => {
            if (type === 'position') {
                worldStore.currentNode = data.node;
            }

            if (type === 'movement') {
                mapService.setActivePath(data.nodes, data.movetime);
                renderService.setActivePath(data.nodes);
            }
        });
    }
}

export const playerService = new PlayerService();
