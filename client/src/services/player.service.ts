import {dataService} from './data.service'
import {mapService} from './map.service'
import {renderService} from './render.service'
import {playerStore} from '../stores/player.store'
import {worldStore} from '../stores/world.store'
import {pathRenderService} from "@/services/path-render.service";
import {SystemTypes} from "@/typegen";

class PlayerService {
    public setup() {
        dataService.subscribe(SystemTypes.Join, (type: number, data: any) => {
            playerStore.isAuthenticated = true;
            playerStore.name = data.name;
            playerStore.bits = data.credits;
            worldStore.currentNode = data.node;
        });

        dataService.subscribe(SystemTypes.Movement, (type: number, data: any) => {
            if (type === SystemTypes.MovementTypes.MoveDone) {
                worldStore.currentNode = data.node;
            }

            if (type === SystemTypes.MovementTypes.StartMove) {
                mapService.setActivePath(data.nodes, data.moveTime);
                pathRenderService.setActivePath(data.nodes);
            }
        });
    }
}

export const playerService = new PlayerService();
