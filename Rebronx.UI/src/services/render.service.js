var instance = null;

export default class RenderService {
	constructor() {
		if (!instance) {
			this.loaded = false;
			this.camera = null;
			this.scene = null;
			this.renderer = null;

			this.canvasW = 200;
			this.canvasH = 200;
		}

		return instance;
	}

	preload() {
		return new Promise(resolve => {
			if (this.loaded == true)
				return resolve();

			this.camera = new THREE.PerspectiveCamera(45, this.canvasW / this.canvasH, 0.01, 2000);
			this.camera.position.x = 1;

			this.scene = new THREE.Scene();
			this.scene.add(this.camera);
			
			this.scene.add(new THREE.AmbientLight(0xffffff, 0.5));
			
			var directionalLight = new THREE.DirectionalLight(0xffffff, 0.5);
			directionalLight.position.set(1.02, 0.97, 0.96);
			this.scene.add(directionalLight);

			this.renderer = new THREE.WebGLRenderer({ alpha: true });
			this.renderer.setPixelRatio(window.devicePixelRatio);
			this.renderer.setSize(this.canvasW, this.canvasH);

			var mtlLoader = new THREE.MTLLoader();
			mtlLoader.load('/assets/items/items.mtl', (materials) => {
				materials.preload();

				var objLoader = new THREE.OBJLoader();
				objLoader.setMaterials(materials);
				objLoader.load('/assets/items/items.obj', (object) => {
					this.scene.add(object);
					this.loaded = true;
					resolve();
				});
			});
		});
	}

	renderItem(meshName) {
		return new Promise(resolve => this.preload().then(() => {
			this.scene.traverse(function (child) {
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