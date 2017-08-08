var instance = null;

export default class PlayerService {
	constructor() {
		if (!instance) {
			instance = this;

			this.player = {
				name: "0",
				credits:0,
				position: { x:0, y:0, z:0, isonground: true }
			};

			dataService.subscribe('join', function(type, data) {
				instance.name = data.name;
				instance.credits = data.credits;
				instance.position = data.position;
			});

			dataService.subscribe('player', function(type, data) {
				if (type === 'position') {
					instance.position = data.position;
				}				
			});
		}

		return instance;
	}


}