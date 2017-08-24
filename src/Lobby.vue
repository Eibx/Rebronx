<template>
	<div class="lobby">
		<ul class="lobby__players">
			<li class="lobby__player" v-for="player in players" v-on:click="showPlayer(player.id)">
				<b>{{player.name}}</b> <i>({{player.health}}/100)</i>
				<div class="lobby__player-info">
					<b>{{player.name}}</b>
					<ul>
						<li>Something</li>
						<li>Something</li>
						<li>Something</li>
					</ul>
				</div>
			</li>
		</ul>

		<div class="playercard" v-bind:class="{ visible: showCard }">
			<div class="playercard__name-container">
				<h2 class="playercard__name">{{cardPlayer.name}}</h2>
			</div>
			<div class="playercard__container">
				{{cardPlayer.health}}
			</div>
			
		</div>
	</div>
</template>

<script>
import DataService from './services/data.service.js'
export default {
	name: 'chat',

	data() {
		return {
			players: [],
			showCard: false,
			cardPlayer: {},
		}
	},
	created() {
		var self = this;
		dataService.subscribe('lobby', (type, data) => {
			if (type == "lobby") {
				this.players = data.players;
			}
		});

		window.addEventListener('hide-player-card', () => { this.showCard = false; });
	},
	methods: {
		showPlayer(id) {
			this.showCard = true;
			
			for (var i = 0; i < this.players.length; i++) {
				if (this.players[i].id == id) {
					this.cardPlayer = this.players[i];
				}
			}
		}
	}
}
</script>

<style>
.lobby {
	position:relative;
}

.lobby__players {
	padding:5px;
}
.lobby__player {
	position:relative;
	display:block;

	background:#171b1f;
	color:#fff;
	padding:5px;
	border-bottom:1px solid #fff;
}
.lobby__player-info {
	width:150px;
	height:100px;
	top:0;
	left:-150px;
	position:absolute;
	padding:10px;
	background:#333;
	color:#fff;
	display:none;
}

.lobby__player:hover {
	background:#222;
	cursor:pointer;
}

.playercard {
	display:none;

	width:200px;
	height:300px;
	position:absolute;
	z-index:10;
	top:20px;
	left:-220px;
	background:#fff;
	color:#000;
	padding:0px;
}
.playercard.visible {
	display:block;
}

.playercard__name-container {
	height:60px;
	display:flex;
	align-items: center;
	justify-content: center;
	background:#ddd;
}
.playercard__name {
	margin:0 0 0 0;
	text-align:center;	
}

.playercard__container {
	padding:20px;
}
</style>
