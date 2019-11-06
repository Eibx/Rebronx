import * as THREE from 'three'
import {mapService} from './map.service';
import {renderService} from './render.service';
import {Color} from "three";

class PathRenderService {
    public activePath: any[] = [];
    public activePathMesh: any;
    public activePathFullMesh: any;

    public setup() : void {

    }

    public setActivePath(paths: number[]) {
        if (this.activePathMesh !== null)
            renderService.scene.remove(this.activePathMesh);

        if (this.activePathFullMesh !== null)
            renderService.scene.remove(this.activePathFullMesh);

        this.activePath = [];
        for (let i = 0; i < paths.length; i++) {
            const node = mapService.getNode(paths[i]);

            if (node)
                this.activePath.push({ x: node.x, y: -node.y });
        }

        function createPath(color: string, lineWidth: number) : THREE.Line {
            const positions = new Float32Array(paths.length * 3);
            for (let i = 0; i < paths.length; i++) {
                const node = mapService.getNode(paths[i]);

                if (node === null)
                    continue;

                positions[i*3] = node.x;
                positions[i*3+1] = 0.1;
                positions[i*3+2] = -node.y;
            }

            const geometry = new THREE.BufferGeometry();
            geometry.addAttribute('position', new THREE.BufferAttribute(positions, 3));
            const material = new THREE.LineBasicMaterial({ color: color, linewidth: lineWidth });
            return new THREE.Line(geometry, material);
        }

        this.activePathFullMesh = createPath("#bbb", 2);
        this.activePathMesh = createPath("#fff", 3);

        renderService.scene.add(this.activePathFullMesh);
        renderService.scene.add(this.activePathMesh);
    }

    public render() {
        let currentStep = mapService.getActiveStep();
        if (currentStep != null) {
            const i = currentStep.index;
            const diffX = (this.activePath[i].x - this.activePath[i-1].x);
            const diffY = (this.activePath[i].y - this.activePath[i-1].y);

            const geometry = this.activePathMesh.geometry;
            geometry.setDrawRange(0, i+1);
            geometry.attributes.position.array[i*3] = this.activePath[i-1].x + diffX * currentStep.travel;
            geometry.attributes.position.array[i*3+2] = this.activePath[i-1].y + diffY * currentStep.travel;
            geometry.attributes.position.needsUpdate = true;

            const geometry2 = this.activePathFullMesh.geometry;
            geometry2.setDrawRange(i-1, this.activePath.length);
            geometry2.attributes.position.array[(i-1)*3] = this.activePath[i-1].x + diffX * currentStep.travel;
            geometry2.attributes.position.array[(i-1)*3+2] = this.activePath[i-1].y + diffY * currentStep.travel;
            geometry2.attributes.position.needsUpdate = true;
        } else {
            renderService.scene.remove(this.activePathMesh);
            renderService.scene.remove(this.activePathFullMesh);
        }
    }
}

export const pathRenderService = new PathRenderService();