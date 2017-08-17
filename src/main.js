import Vue from 'vue'
import Chat from './Chat.vue'
import Login from './Login.vue'
import Map from './Map.vue'
import Lobby from './Lobby.vue'

import DataService from './services/data.service.js'
import PlayerService from './services/player.service.js'

window.dataService = new DataService();
window.playerService = new PlayerService();

new Vue({ el: '#chat', render: r => r(Chat) });
new Vue({ el: '#login', render: r => r(Login) });
new Vue({ el: '#map', render: r => r(Map) });
new Vue({ el: '#lobby', render: r => r(Lobby) });