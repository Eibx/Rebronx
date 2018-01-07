var instance = null;

export default class PlayerService {
	constructor() {
		if (!instance) {
			this.player = {
				name: "0",
				credits:0,
				position: { location: 0, dimention: 0 }
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

			instance = this;
		}

		return instance;
	}


}