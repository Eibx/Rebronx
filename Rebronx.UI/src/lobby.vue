<template>
	<div class="lobby">
		<ul class="lobby__players">
			<li class="lobby__player" v-for="player in players">
				<img class="lobby__player-avatar" />
				<div class="lobby__player-info">
					<span class="lobby__player-name">{{player.name}}</span>
				</div>
			</li>
		</ul>
	</div>
</template>

<script>
import DataService from './services/data.service.js'
export default {
	name: 'chat',

	data() {
		return {
			players: [],
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
	display:flex;
}
.lobby__player {
	position:relative;
	display:flex;
	width:50%;
	background:#171b1f;
	border:1px solid #171b1f;
	color:#fff;
}
.lobby__player:nth-child(odd) {
	margin-right:5px
}

.lobby__player-info {
	padding:10px;
}

.lobby__player-name {

}
.lobby__player-avatar {
	height:50px;
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
