class DataService {
    public subscribers: { [key: string]: any } = {};
    public websocket: WebSocket | null;

    constructor() {
        this.subscribers = {};
        this.websocket = null;
    }

    onData(message: any) {
        if (message.data == "pong")
            return;

        console.log("received:", message.data);

        let baseMessage = null;

        try {
            baseMessage = JSON.parse(message.data);
        } catch(e) {
            return;
        }

        const subscriptions = this.subscribers[baseMessage.component];
        if (subscriptions) {
            for (let i = 0; i < subscriptions.length; i++) {
                subscriptions[i](baseMessage.type, baseMessage.data);
            }
        }
    }

    open(callback: Function) {
        this.websocket = new WebSocket("wss://" + window.location.hostname + ":21220");
        this.websocket.onmessage = (msg) => { this.onData(msg) };
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

    subscribe(name: string, callback: Function) {
        if (this.subscribers[name])
            this.subscribers[name].push(callback);
        else
            this.subscribers[name] = [callback];
    }

    send(component: string, type: string, data: object) {
        try {
            var jsonData = JSON.stringify(data);
            var json = JSON.stringify({ component: component, type: type, data: jsonData });
            console.log("sent: ", json);

            if (this.websocket !== null)
                this.websocket.send(json);
        } catch (e) {
            console.error(e);
        }
    }
}

export const dataService = new DataService();