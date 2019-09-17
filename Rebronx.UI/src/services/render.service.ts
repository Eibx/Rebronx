import * as THREE from 'three'
import GLTFLoader from 'three-gltf-loader'

class RenderService {
    public loaded:boolean = false;
    public camera:any;
    public scene:any;
    public renderer:any;

    public canvasW: number = 500;
    public canvasH: number = 500;

    preload() {
        return new Promise((resolve) => {
            if (this.loaded == true)
                return resolve();

            this.camera = new THREE.PerspectiveCamera(90, this.canvasW / this.canvasH, 0.01, 10000);
            this.camera.position.set(2, 10, 15);

            this.scene = new THREE.Scene();
            this.scene.add(this.camera);
            
            this.scene.add(new THREE.AmbientLight(0xffffff, 0.5));
            
            var directionalLight = new THREE.DirectionalLight(0xffffff, 0.5);
            directionalLight.position.set(1.02, 0.97, 0.96);
            this.scene.add(directionalLight);

            this.renderer = new THREE.WebGLRenderer({ alpha: true });
            this.renderer.setPixelRatio(window.devicePixelRatio);
            this.renderer.setSize(this.canvasW, this.canvasH);

            const loader = new GLTFLoader();
            loader.load('/assets/city.glb', (glb) => {
                this.scene.add(glb.scene);
                this.loaded = true;
                resolve();
            });
        });
    }

    renderMap() : Promise<string> {
        return new Promise((resolve) => this.preload().then(() => {
            this.renderer.setClearColor(0x000000, 0);

            this.camera.lookAt(this.scene.position);
            this.renderer.render(this.scene, this.camera);
            resolve(this.renderer.domElement.toDataURL());
        }));
    }

    renderItem(meshName:string) : Promise<string> {
        return new Promise((resolve) => this.preload().then(() => {
            this.scene.traverse(function (child:any) {
                if (child instanceof THREE.Mesh) {
                    child.visible = child.name === meshName.toString();
                }
            });

            this.renderer.setClearColor(0x000000, 0);

            this.camera.lookAt(this.scene.position);
            this.renderer.render(this.scene, this.camera);
            resolve(this.renderer.domElement.toDataURL());
        }));
    }
}

export default new RenderService();