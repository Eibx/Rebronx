import * as THREE from 'three'
import {worldRenderService} from "@/services/world-render.service";
import {pathRenderService} from "@/services/path-render.service";

class RenderService {
    public camera: any = null;
    public scene: any;
    public renderer: any;
    public raycaster = new THREE.Raycaster();

    public canvasW: number = 500;
    public canvasH: number = 500;

    private plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0);
    public cursorPosition = new THREE.Vector2(0, 0);

    public setup() : HTMLCanvasElement {
        this.canvasW = document.body.clientWidth;
        this.canvasH = document.body.clientHeight;

        this.camera = new THREE.PerspectiveCamera(90, this.canvasW / this.canvasH, 0.01, 10000);
        this.camera.position.set(0, 15, 10);

        this.scene = new THREE.Scene();
        this.scene.add(this.camera);

        worldRenderService.setup();
        pathRenderService.setup();

        // Render
        this.renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
        this.renderer.setPixelRatio(window.devicePixelRatio);
        this.renderer.setSize(this.canvasW, this.canvasH);

        this.renderer.setClearColor(0x000000, 0);

        this.camera.lookAt(this.scene.position);

        requestAnimationFrame(() => this.render());

        return this.renderer.domElement;
    }

    public resize() {
        this.canvasW = document.body.clientWidth;
        this.canvasH = document.body.clientHeight;
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(this.canvasW, this.canvasH);
    }

    public onMouseMove(event:MouseEvent) {
        const x = (event.clientX / window.innerWidth) * 2 - 1;
        const y = -(event.clientY / window.innerHeight) * 2 + 1;

        this.raycaster.setFromCamera(new THREE.Vector2(x, y), this.camera);

        let intersects = new THREE.Vector3();
        this.raycaster.ray.intersectPlane(this.plane, intersects);

        this.cursorPosition = new THREE.Vector2(intersects.x, intersects.z);
    }

    public render() {
        if (this.camera === null)
            return;

        worldRenderService.render();
        pathRenderService.render();

        this.renderer.render(this.scene, this.camera);

        requestAnimationFrame(() => this.render());
    }
}

export const renderService = new RenderService();