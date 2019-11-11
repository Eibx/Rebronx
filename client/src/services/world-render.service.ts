import * as THREE from 'three'
import GLTFLoader from 'three-gltf-loader'
import {mapService} from './map.service';
import {worldStore} from "@/stores/world.store";
import {renderService} from "@/services/render.service";

const tailwindConfig = require('@/../tailwind.config.js');

class WorldRenderService {
    public mapModel: any = null;
    private cones: { [id: string] : any; } = {};

    public setup() : void {
        this.preloadModel().then(() => {
            const scene = this.mapModel.scene;

            scene.traverse((child: any) => {
                if (child instanceof THREE.Mesh) {
                    let grays = tailwindConfig.theme.extend.colors.gray;
                    let color = grays["700"];

                    if (child.name == "grid") {
                        color = grays["800"];
                    } else if (child.name == "pavement") {
                        color = grays["700"];
                    } else if (child.name == "roads") {
                        color = grays["600"];
                    }

                    child.material = new THREE.MeshBasicMaterial({ color: color });
                }
            });

            renderService.scene.add(scene);

            // Location cones
            const nodes = mapService.getAllLocations();
            for (let i = 0; i < nodes.length; i++) {
                const node = nodes[i];
                let geometry = new THREE.ConeGeometry(0.20, 1.0, 4);
                let material = new THREE.MeshBasicMaterial({ color: 0x00000000 });
                let cone = new THREE.Mesh(geometry, material);
                cone.position.set(node.x, 0.5, node.y);

                this.cones[parseInt(node.id)] = cone;
                renderService.scene.add(cone);
            }
        });
    }

    public render() {
        let reds = tailwindConfig.theme.extend.colors.red;
        let grays = tailwindConfig.theme.extend.colors.gray;
        let blues = tailwindConfig.theme.extend.colors.blue;

        for (let id in this.cones) {
            let coneColor = this.cones[id].material.color;
            coneColor.setStyle(reds["500"]);

            if (worldStore.currentNode.toString() === id)
                coneColor.setStyle(blues["500"]);
        }

        let closeNode = mapService.getCloseLocation(renderService.cursorPosition.x, renderService.cursorPosition.y);
        if (closeNode !== null) {
            this.cones[closeNode.id].material.color.setStyle(reds["100"]);
        }
    }

    private preloadModel() {
        return new Promise((resolve) => {
            if (this.mapModel !== null)
                return resolve();

            const loader = new GLTFLoader();
            loader.load('/assets/city.glb', (glb) => {
                this.mapModel = glb;
                resolve();
            });
        });
    }
}

export const worldRenderService = new WorldRenderService();