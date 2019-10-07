import * as THREE from 'three'
import GLTFLoader from 'three-gltf-loader'
import mapService from './map.service';

class RenderService {
    public camera: any = null;
    public scene: any;
    public renderer: any;
    public raycaster = new THREE.Raycaster();;

    public canvasW: number = 500;
    public canvasH: number = 500;

    public mapModel: any = null;
    
    private cones: { [id: string] : any; } = {};

    private plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0);
    public cursorPosition = new THREE.Vector2(0, 0);
    public activePath: any[] = [];
    public activePathMesh: any;
    public startTime: number = 0;
    public endTime: number = 0;

    public setup() : Promise<HTMLCanvasElement> {
        return new Promise<HTMLCanvasElement>((resolve) => this.preloadModel().then(() => {
            this.canvasW = document.body.clientWidth;
            this.canvasH = document.body.clientHeight;

            this.camera = new THREE.PerspectiveCamera(90, this.canvasW / this.canvasH, 0.01, 10000);
            this.camera.position.set(0, 15, 10);

            this.scene = new THREE.Scene();
            this.scene.add(this.camera);
            this.scene.add(this.mapModel.scene);

            let light = new THREE.PointLight(0xffffff, 1, 1000);
            light.position.set(50, 50, 50);
            this.scene.add(light);

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
        const x = (event.clientX / window.innerWidth) * 2 - 1;
        const y = -(event.clientY / window.innerHeight) * 2 + 1;
        
        this.raycaster.setFromCamera(new THREE.Vector2(x, y), this.camera);

        let intersects = new THREE.Vector3();
        this.raycaster.ray.intersectPlane(this.plane, intersects);

        this.cursorPosition = new THREE.Vector2(intersects.x, -intersects.z);
    }

    public setActivePath(paths: number[]) {
        if (this.activePathMesh !== null) {
            this.scene.remove(this.activePathMesh);
        }

        this.startTime = new Date().getTime();
        this.endTime = new Date().getTime() + 10000;
        this.activePath = [];
        var positions = new Float32Array(paths.length * 3);
        for (let i = 0; i < paths.length; i++) {
            const node = mapService.getNode(paths[i]);
            if (node === null)
                continue;

            this.activePath.push({ x: node.x, y: -node.y });
            positions[i*3+0] = node.x;
            positions[i*3+1] = 0.2;
            positions[i*3+2] = -node.y;
        }

        var geometry = new THREE.BufferGeometry();
        geometry.addAttribute('position', new THREE.BufferAttribute(positions, 3));

        var material = new THREE.LineBasicMaterial({ color: 0x0000ff, linewidth: 3 });
        var line = new THREE.Line(geometry, material);
        this.activePathMesh = line;

        this.scene.add(line);
    }

    public render() {
        if (this.camera === null)
            return;

        for (let id in this.cones) {
            this.cones[id].material.color.setHex(0xE06C75);
        }

        let closeNode = mapService.getCloseLocation(this.cursorPosition.x, this.cursorPosition.y);
        if (closeNode !== null) {
            this.cones[closeNode.id].material.color.setHex(0xFFD1D5);
        }

        if (this.activePath.length > 1) {
            const geometry = this.activePathMesh.geometry
            const segmentTime = (this.endTime - this.startTime) / (this.activePath.length-1);

            let currentTime = (new Date().getTime() - this.startTime);
            if (this.startTime > this.endTime)
                currentTime = (this.endTime - this.startTime);

            let segment = Math.ceil(currentTime / segmentTime);
            if (segment == 0) {
                segment = 1;
            }
            geometry.setDrawRange(0, segment+1);

            const l = segment;
            const diffX = (this.activePath[l].x - this.activePath[l-1].x)
            const diffY = (this.activePath[l].y - this.activePath[l-1].y)
            const s = (currentTime-segmentTime*(segment-1)) / segmentTime;
            
            geometry.attributes.position.array[l*3+0] = this.activePath[l-1].x + diffX*s;
            geometry.attributes.position.array[l*3+2] = this.activePath[l-1].y + diffY*s;
            geometry.attributes.position.needsUpdate = true;
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

export default new RenderService();