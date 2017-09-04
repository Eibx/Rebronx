<template>
	<div class="map" v-on:mousedown="mousedown" v-on:mouseup="mouseup" v-on:mousemove="mousemove" v-on:wheel="mousewheel">
		<svg viewBox="0 0 1000 1000">
			<image x="0" y="0" width="1000" height="1000" xlink:href="/assets/map.svg" />
			<template v-for="node in mapLines">
				<line v-bind:x1="node.sx * 10" v-bind:y1="node.sy * 10" v-bind:x2="node.ex * 10" v-bind:y2="node.ey * 10" stroke-width="2" stroke="#666" />
				<line v-bind:x1="node.sx * 10" v-bind:y1="node.sy * 10" v-bind:x2="((node.ex-node.sx)*movePercentage+node.sx)*10" v-bind:y2="((node.ey-node.sy)*movePercentage+node.sy)*10" stroke-width="2" stroke="#fff" />
			</template>
			<template v-for="node in mapData">
				<circle class="map__node" r="6" v-bind:cx="node.posX*10" v-bind:cy="node.posY*10" v-bind:class="{ 'map__node-current': node.isCurrent }" v-on:click="move(node.id)" />
			</template>
		</svg>
	</div>
</template>

<script>
import Dijkstra from './helper/dijkstra.js'

export default {
	name: 'map',

	data() {
		return {
			mapData: [],
			mapLines: [],
			movementPath: [],
			dijkstra: null,

			isDown: false,
			startX: 0,
			startY: 0,
			offsetX: 0,
			offsetY: 0,
			movePercentage: 0.5,
			moveX: 0,
			moveY: 0,
			zoomW: 500,
		}
	},
	created() {
		var movementTimeout;
		var movementCount;

		this.mapData = window.mapData.map;
		var pathData = {};
		for (var i = 0; i < this.mapData.length; i++) {
			var element = this.mapData[i];
			pathData[element.id] = {};

			for (var j = 0; j < element.connections.length; j++) {
				pathData[element.id.toString()][element.connections[j]] = this.getDistance(this.mapData, element.id, element.connections[j]);				
			}
		}

		this.dijkstra = new Dijkstra(pathData);

		dataService.subscribe('player', (type, data) => {
			if (type == "position") {
				this.setCurrentPosition(data.position);
				clearInterval(movementTimeout);

				this.movePercentage = 0;
				this.mapLines = [];

				if (this.movementPath.length > 0) {
					dataService.send('movement', 'move', { position: this.movementPath.shift() });
				}
			}
			else if (type == "movement") {
				var curpos = this.getPointPosition(this.mapData, playerService.position);
				var newpos = this.getPointPosition(this.mapData, data.position);

				this.mapLines = [];
				this.mapLines.push({ sx: curpos.x, sy: curpos.y, ex: newpos.x, ey: newpos.y });

				this.movePercentage = 0;
				clearInterval(movementTimeout);
				movementTimeout = setInterval(() => { this.movePercentage += 1 /(data.movetime / 10); }, 10)
			}
		});

		dataService.subscribe('join', (type, data) => {
			if (type == "join") {
				this.setCurrentPosition(data.position);
			}
		});
	},
	methods: {
		move: function(id) {
			this.movementPath = this.dijkstra.findShortestPath(playerService.position, id);
			this.movementPath.shift();
			dataService.send('movement', 'move', { position: this.movementPath.shift() });
		},
		mousedown: function (evt) {
			evt.preventDefault();
			this.startX = evt.clientX;
			this.startY = evt.clientY;
			this.moveX = 0;
			this.moveY = 0;
			this.isDown = true;

			window.dispatchEvent(new Event('hide-player-card'));
		},
		mouseup: function (evt) {
			evt.preventDefault();
			this.offsetX += evt.clientX - this.startX;
			this.offsetY += evt.clientY - this.startY;
			this.moveX = 0;
			this.moveY = 0;
			this.isDown = false;
		},
		mousemove: function (evt) {
			evt.preventDefault();
			if (this.isDown) {
				this.moveX = evt.clientX - this.startX;
				this.moveY = evt.clientY - this.startY;
			}
		},
		mousewheel: function (evt) {
			evt.preventDefault();
			var direction = 0;
			if (evt.deltaY < 0) {
				direction = 1;
			} else {
				direction = -1;
			}

			var zoomAmount = this.zoomW * 0.10;

			var perX = (evt.clientX - this.offsetX) / this.zoomW;
			var perY = (evt.clientY - this.offsetY) / this.zoomW;

			this.zoomW += (zoomAmount * direction * (1 - perY)) + ((zoomAmount * direction) * (1 - perX));
			this.offsetX -= (zoomAmount * direction) * perX;
			this.offsetY -= (zoomAmount * direction) * perY;
		},

		getPointPosition(mapData, id) {
			for (var i = 0; i < mapData.length; i++) {
				var p = mapData[i];
				if (p.id == id) {
					return { x: p.posX, y: p.posY };
				}
			}
		},
		setCurrentPosition(position) {
			for (var i = 0; i < this.mapData.length; i++) {
				this.$set(this.mapData[i], 'isCurrent', (this.mapData[i].id == position));
			}
		},
		getDistance(mapData, first, second) {
			var a = this.getPointPosition(mapData, first);
			var b = this.getPointPosition(mapData, second);

			var aa = (b.x-a.x);
			var bb = (b.y-a.y);

			return Math.abs(Math.sqrt(aa*aa + bb*bb));
		}
	}
}
</script>

<style scoped>
.map {
	user-select: none;
	position: absolute;
	width: 100%;
	height: 100%;
	left: 0;
	top: 0;
	z-index: 1;
}

.map img {
	position: absolute;
}
.map__node {
	stroke:transparent;
	stroke-width:20px;
}
.map__node:hover {
	fill:yellowgreen;
}
.map__node-current {
	fill:red;
}
</style>
