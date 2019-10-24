class PlayerStore {
    public isAuthenticated: boolean = false;
    public name: string = "";
    public bits: number = 0;
}

const playerStore = new PlayerStore();

export default playerStore;