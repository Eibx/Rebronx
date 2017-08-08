import Vue from 'vue'
import Chat from './Chat.vue'
import Login from './Login.vue'
import Map from './Map.vue'

import DataService from './services/data.service.js'
import PlayerService from './services/player.service.js'

window.dataService = new DataService();
window.playerService = new PlayerService();

new Vue({
	el: '#chat',
	render: x => x(Chat)
})

new Vue({
	el: '#login',
	render: x => x(Login)
})

new Vue({
	el: '#map',
	render: x => x(Map)
})
