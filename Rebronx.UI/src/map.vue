<template>
	<div class="map" v-on:mousedown="mousedown" v-on:mouseup="mouseup" v-on:mousemove="mousemove" v-on:wheel="mousewheel">
		<svg viewBox="0 0 800 800">
			<image x="0" y="0" width="800" height="800" xlink:href="/assets/map2_original.svg" />
			<template v-for="(node, index) in mapLines">
				<line v-bind:x1="node.sx" v-bind:y1="node.sy" v-bind:x2="node.ex" v-bind:y2="node.ey" stroke-width="2" stroke="#999" />
				<line v-bind:x1="node.sx" v-bind:y1="node.sy" v-bind:x2="((node.ex-node.sx)*movePercentage+node.sx)" v-bind:y2="((node.ey-node.sy)*movePercentage+node.sy)" v-if="index == 0" stroke-width="2" stroke="#ffff" />
			</template>
			<template v-for="node in mapData">
				<text v-bind:x="node.posX + 3" v-bind:y="node.posY + 10">{{node.id}}</text>
				<circle class="map__node-current" r="4" v-if="node.isCurrent" v-bind:cx="node.posX" v-bind:cy="node.posY" />
				<circle class="map__node" r="2" v-bind:cx="node.posX" v-bind:cy="node.posY" v-on:click="move(node.id)" />
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
			grid: [],

			isDown: false,
			startX: 0,
			startY: 0,
			offsetX: 0,
			offsetY: 0,
			movePercentage: 0.5,
			moveX: 0,
			moveY: 0,
			zoomW: 500
		}
	},
	created() {
		var self = this;

		var dijkstraData = {};
		
		this.mapData = window.mapData.map;

		for (var i = 0; i < this.mapData.length; i++) {
			var element = this.mapData[i];
			this.$set(element, "isCurrent", false);
			dijkstraData[element.id] = {};

			for (var j = 0; j < element.connections.length; j++) {
				dijkstraData[element.id.toString()][element.connections[j]] = getDistance(this.mapData, element.id, element.connections[j]);
			}
		}

		this.dijkstra = new Dijkstra(dijkstraData);

		function getPointPosition(mapData, id) {
			for (var i = 0; i < mapData.length; i++) {
				var p = mapData[i];
				if (p.id == id) {
					return { x: p.posX, y: p.posY };
				}
			}
		}

		function setCurrentPosition(position) {
			for (var i = 0; i < self.mapData.length; i++) {
				self.mapData[i].isCurrent = (self.mapData[i].id == position.x);
			}
		}

		function getDistance(mapData, first, second) {
			var a = getPointPosition(mapData, first);
			var b = getPointPosition(mapData, second);

			var aa = (b.x-a.x);
			var bb = (b.y-a.y);

			return Math.abs(Math.sqrt(aa*aa + bb*bb));
		}

		var movementTimeout;
		var movementCount;
		dataService.subscribe('player', (type, data) => {
			if (type == "position") {
				setCurrentPosition(data.position);
				clearInterval(movementTimeout);

				self.movePercentage = 0;

				if (this.movementPath.length > 0) {
					dataService.send('movement', 'move', { position: this.movementPath.shift() });
				} else {
					this.mapLines = [];
				}
			}
			else if (type == "movement") {
				self.mapLines = [];

				var m = this.movementPath.slice(0);
				m.unshift(playerService.position.x, data.position.x);
				
				for (var i = 0; i < m.length-1; i++) {
					var s = getPointPosition(this.mapData, m[i]);
					var e = getPointPosition(this.mapData, m[i+1]);
					self.mapLines.push({ sx: s.x, sy: s.y, ex: e.x, ey: e.y });
				}

				self.movePercentage = 0;
				clearInterval(movementTimeout);
				movementTimeout = setInterval(() => { 
					self.movePercentage += 1 /(data.movetime / 10); 
					if (self.movePercentage > 1) {
						self.movePercentage = 1;
						clearInterval(movementTimeout);
					}
				}, 10);
			}
		});
		dataService.subscribe('join', function(type, data) {
			if (type == "join") {
				setCurrentPosition(data.position);
			}
		});
	},
	methods: {
		move: function(id) {
			this.movementPath = this.dijkstra.findShortestPath(playerService.position.x, id);
			this.movementPath.shift();
			dataService.send('movement', 'move', { position: { x: this.movementPath.shift(), y: 0 }});
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
	stroke-width:8px;
}
.map__node:hover {
	fill:yellowgreen;
}
.map__node-current {
	fill:transparent;
	stroke:#ffffff99;
	stroke-width:1px;
}
</style>