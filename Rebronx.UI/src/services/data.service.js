var instance = null;

export default class DataService {
	constructor() {
		if (!instance) {
			instance = this;
		}

		this.subscribers = {};
		this.websocket = null;

		return instance;
	}

	onData(message) {
		if (message.data == "pong")
			return;

		console.log("received:", message.data);

		var baseMessage = null
		try {
			baseMessage = JSON.parse(message.data);
		} 
		catch(e) 
		{ 
			return;
		}

		var subscriptions = this.subscribers[baseMessage.component];
		if (subscriptions) {
			for (var i = 0; i < subscriptions.length; i++) {
				subscriptions[i](baseMessage.type, baseMessage.data);
			}
		}
	}

	open(callback) {
		this.websocket = new WebSocket("ws://localhost:31337");
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
		setInterval(() => { this.websocket.send("ping"); }, 5000);
	}

	subscribe(name, callback) {
		if (this.subscribers[name])
			this.subscribers[name].push(callback);
		else
			this.subscribers[name] = [callback];

		console.info('@Subscriber added: ' + name);
	}

	send(component, type, data) {
		try {
			var jsonData = JSON.stringify(data);
			var json = JSON.stringify({ component: component, type: type, data: jsonData });
			console.log("sent:    ", json);
			this.websocket.send(json);
		} catch (e) {
			console.error(e);
		}
	}
}