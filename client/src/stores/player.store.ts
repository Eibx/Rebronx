class PlayerStore {
    public isAuthenticated: boolean = false;
    public name: string = "";
    public bits: number = 0;
}

export const playerStore = new PlayerStore();