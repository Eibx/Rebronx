import DataService from './data.service'

class PlayerService {
    public player: any = {
        name: "0",
        credits:0,
        position: { location: 0, dimention: 0 }
    };

    constructor() {
        DataService.subscribe('join', (type: any, data: any) => {
            this.player.name = data.name;
            this.player.credits = data.credits;
            this.player.position = data.position;
        });

        DataService.subscribe('player', (type: any, data: any) => {
            if (type === 'position') {
                this.player.position = data.position;
            }
        });
    }
}

export default new PlayerService();
