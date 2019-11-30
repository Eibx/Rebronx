class DataService {
    public subscribers: { [key: string]: any } = {};
    public websocket: WebSocket | null;

    constructor() {
        this.subscribers = {};
        this.websocket = null;
    }

    async onData(message: MessageEvent) {

        let arrayBuffer = await message.data.arrayBuffer();
        let buffer = new Uint8Array(arrayBuffer);

        const system = buffer[0];
        const type = buffer[1];
        let data = "";
        try {
            data = JSON.parse(new TextDecoder().decode(buffer.slice(2)));
            console.log("received: ", "system:" + system, "type:" + type, data);
        } catch (e) {
            return;
        }

        const subscriptions = this.subscribers[system];
        if (subscriptions) {
            for (let i = 0; i < subscriptions.length; i++) {
                subscriptions[i](type, data);
            }
        }
    }

    open(callback: Function) {
        if (this.websocket !== null)
            this.websocket.close();

        this.websocket = new WebSocket("wss://" + window.location.hostname + ":21220");
        this.websocket.onmessage = (msg: MessageEvent) => { this.onData(msg) };
        this.websocket.onopen = () => {
            callback({ type: 'open' });
        };
        this.websocket.onerror = (e) => {
            callback({ type: 'error' });
        };
        this.websocket.onclose = (e) => {
            callback({ type: 'error' });
        };
    }

    startPing() {
        setInterval(() => {
            if (this.websocket !== null)
                this.websocket.send("ping");
        }, 5000);
    }

    subscribe(system: number, callback: (type: number, data: any) => void) {
        if (this.subscribers[system])
            this.subscribers[system].push(callback);
        else
            this.subscribers[system] = [callback];
    }

    send(system: number, type: number, data: object) {
        try {
            let jsonData = JSON.stringify(data);
            console.log("sent: ", "system:" + system, "type:" + type, jsonData);

            const typeBytes = new Uint8Array(2);
            typeBytes[0] = system;
            typeBytes[1] = type;

            const dataBytes = new TextEncoder().encode(jsonData);

            const bytes = new Uint8Array(typeBytes.length + dataBytes.length);
            bytes.set(typeBytes, 0);
            bytes.set(dataBytes, 2);

            if (this.websocket !== null)
                this.websocket.send(bytes);
        } catch (e) {
            console.error(e);
        }
    }
}

export const dataService = new DataService();