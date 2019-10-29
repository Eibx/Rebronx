import * as THREE from 'three'
import GLTFLoader from 'three-gltf-loader'
import {mapService} from './map.service';
import {worldStore} from "@/stores/world.store";

class RenderService {
    public camera: any = null;
    public scene: any;
    public renderer: any;
    public raycaster = new THREE.Raycaster();

    public canvasW: number = 500;
    public canvasH: number = 500;

    public mapModel: any = null;

    private cones: { [id: string] : any; } = {};

    private plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0);
    public cursorPosition = new THREE.Vector2(0, 0);
    public activePath: any[] = [];
    public activePathMesh: any;
    public activePathFullMesh: any;

    public setup() : Promise<HTMLCanvasElement> {
        return new Promise<HTMLCanvasElement>((resolve) => this.preloadModel().then(() => {
            this.canvasW = document.body.clientWidth;
            this.canvasH = document.body.clientHeight;

            this.camera = new THREE.PerspectiveCamera(90, this.canvasW / this.canvasH, 0.01, 10000);
            this.camera.position.set(0, 15, 10);

            this.scene = new THREE.Scene();
            this.scene.add(this.camera);
            this.scene.add(this.mapModel.scene);

            // Setup light
            let light = new THREE.PointLight(0xffffff, 1, 1000);
            light.position.set(50, 50, 50);
            this.scene.add(light);

            // Location cones
            const nodes = mapService.getAllLocations();
            for (let i = 0; i < nodes.length; i++) {
                const node = nodes[i];
                let geometry = new THREE.ConeGeometry(0.10, 0.8, 4);
                let material = new THREE.MeshBasicMaterial({ color: 0xE06C75 });
                let cone = new THREE.Mesh(geometry, material);
                cone.position.set(node.x, 0.5, -node.y);

                this.cones[node.id] = cone;
                this.scene.add(cone);
            }

            // Render
            this.renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
            this.renderer.setPixelRatio(window.devicePixelRatio);
            this.renderer.setSize(this.canvasW, this.canvasH);

            this.renderer.setClearColor(0x000000, 0);

            this.camera.lookAt(this.scene.position);

            requestAnimationFrame(() => this.render());

            resolve(this.renderer.domElement);
        }));
    }

    public resize() {
        this.canvasW = document.body.clientWidth;
        this.canvasH = document.body.clientHeight;
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(this.canvasW, this.canvasH);
    }

    public onMouseMove(event:MouseEvent) {
        if (this.mapModel === null)
            return;

        const x = (event.clientX / window.innerWidth) * 2 - 1;
        const y = -(event.clientY / window.innerHeight) * 2 + 1;

        this.raycaster.setFromCamera(new THREE.Vector2(x, y), this.camera);

        let intersects = new THREE.Vector3();
        this.raycaster.ray.intersectPlane(this.plane, intersects);

        this.cursorPosition = new THREE.Vector2(intersects.x, -intersects.z);
    }

    public setActivePath(paths: number[]) {
        if (this.activePathMesh !== null)
            this.scene.remove(this.activePathMesh);

        if (this.activePathFullMesh !== null)
            this.scene.remove(this.activePathFullMesh);

        this.activePath = [];
        const positions = new Float32Array(paths.length * 3);
        for (let i = 0; i < paths.length; i++) {
            const node = mapService.getNode(paths[i]);

            if (node === null)
                continue;

            this.activePath.push({ x: node.x, y: -node.y });
            positions[i*3] = node.x;
            positions[i*3+1] = 0.15;
            positions[i*3+2] = -node.y;
        }

        const positions2 = new Float32Array(paths.length * 3);
        for (let i = 0; i < paths.length; i++) {
            const node = mapService.getNode(paths[i]);

            if (node === null)
                continue;

            this.activePath.push({ x: node.x, y: -node.y });
            positions2[i*3] = node.x;
            positions2[i*3+1] = 0.1;
            positions2[i*3+2] = -node.y;
        }

        const geometry = new THREE.BufferGeometry();
        geometry.addAttribute('position', new THREE.BufferAttribute(positions, 3));
        const material = new THREE.LineBasicMaterial({ color: 0xeeeeee, linewidth: 3 });
        this.activePathMesh = new THREE.Line(geometry, material);

        const geometry2 = new THREE.BufferGeometry();
        geometry2.addAttribute('position', new THREE.BufferAttribute(positions2, 3));
        const material2 = new THREE.LineBasicMaterial({ color: 0x999999, linewidth: 3 });
        this.activePathFullMesh = new THREE.Line(geometry2, material2);

        this.scene.add(this.activePathFullMesh);
        this.scene.add(this.activePathMesh);
    }

    public render() {
        if (this.camera === null)
            return;

        for (let id in this.cones) {
            let coneColor = this.cones[id].material.color;

            if (worldStore.currentNode.toString() === id)
                coneColor.setHex(0x6C75E0);
            else
                coneColor.setHex(0xE06C75);
        }

        let closeNode = mapService.getCloseLocation(this.cursorPosition.x, this.cursorPosition.y);
        if (closeNode !== null) {
            this.cones[closeNode.id].material.color.setHex(0xFFD1D5);
        }

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
        } else {
            this.scene.remove(this.activePathMesh);
            this.scene.remove(this.activePathFullMesh);
        }

        this.renderer.render(this.scene, this.camera);

        requestAnimationFrame(() => this.render());
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

export const renderService = new RenderService();