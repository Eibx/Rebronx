import * as THREE from 'three'
import GLTFLoader from 'three-gltf-loader'

import mapData from '../../../Rebronx.Data/map.json';

class RenderService {
    public camera: any;
    public scene: any;
    public renderer: any;
    public raycaster: any;

    public canvasW: number = 500;
    public canvasH: number = 500;

    public mapModel: any = null;
    
    public mouse = new THREE.Vector2(0, 0);
    public plane: any;

    public mapData = require('../../../Rebronx.Data/map.json');

    public mousePosition = new THREE.Vector3(0, 0, 0);

    public setup() : Promise<HTMLCanvasElement> {
        return new Promise<HTMLCanvasElement>((resolve) => this.preloadModel().then(() => {
            this.canvasW = document.body.clientWidth;
            this.canvasH = document.body.clientHeight;

            this.camera = new THREE.PerspectiveCamera(90, this.canvasW / this.canvasH, 0.01, 10000);
            this.camera.position.set(0, 15, 10);

            this.raycaster = new THREE.Raycaster();

            this.scene = new THREE.Scene();
            this.scene.add(this.camera);
            this.scene.add(this.mapModel.scene);

            this.plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0);

            var light = new THREE.PointLight(0xffffff, 1, 1000);
            light.position.set(50, 50, 50);
            this.scene.add(light);

            for (var i = 0; i < mapData.map.length; i++) {
                var geometry = new THREE.ConeGeometry(0.10, 0.8, 4);
                var material = new THREE.MeshBasicMaterial({ color: 0xE06C75 });
                var cone = new THREE.Mesh(geometry, material);
                mapData.map[i].model = cone;
                cone.position.set(mapData.map[i].x, 0.5, -mapData.map[i].y);
                this.scene.add(cone);
            }

            this.renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
            this.renderer.setPixelRatio(window.devicePixelRatio);
            this.renderer.setSize(this.canvasW, this.canvasH);

            this.renderer.setClearColor(0x000000, 0);

            this.camera.lookAt(this.scene.position);

            requestAnimationFrame(() => this.render());

            resolve(this.renderer.domElement);
        }));
    }

    public getMousePosition(): THREE.Vector3 {
        this.raycaster.setFromCamera(this.mouse, this.camera);
        var intersects = new THREE.Vector3();
        this.raycaster.ray.intersectPlane(this.plane, intersects);

        return intersects;
    }

    public resize() {
        this.canvasW = document.body.clientWidth;
        this.canvasH = document.body.clientHeight;
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(this.canvasW, this.canvasH);
    }

    public render() {
        this.renderer.render(this.scene, this.camera);

        for (let i = 0; i < this.mapData.map.length; i++) {
            const cone = this.mapData.map[i].model;
            if (Math.abs(cone.position.x-this.mousePosition.x) < 0.75 && Math.abs(cone.position.z-this.mousePosition.z) < 0.75) {
                cone.material.color.setHex(0xFFD1D5);
            } else {
                cone.material.color.setHex(0xE06C75);
            }
        }

        requestAnimationFrame(() => this.render());
    }

    public onMouseMove(event:MouseEvent) {
        this.mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
        this.mouse.y = -(event.clientY / window.innerHeight) * 2 + 1;
        this.mousePosition = this.getMousePosition();
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

export default new RenderService();