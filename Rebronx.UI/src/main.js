import Vue from 'vue'
import Chat from './chat.vue'
import Login from './login.vue'
import Map from './map.vue'
import Lobby from './lobby.vue'
import Inventory from './inventory.vue'

import DataService from './services/data.service.js'
import PlayerService from './services/player.service.js'

window.mapData = require("../../Rebronx.Data/map.json");
window.itemData = require("../../Rebronx.Data/items.json");

window.dataService = new DataService();
window.playerService = new PlayerService();

new Vue({ el: '#chat', render: r => r(Chat) });
new Vue({ el: '#login', render: r => r(Login) });
new Vue({ el: '#map', render: r => r(Map) });
new Vue({ el: '#lobby', render: r => r(Lobby) });
new Vue({ el: '#inventory', render: r => r(Inventory) });

window.addEventListener('keyup', function(evt) {
    var elm = evt.target.tagName.toLowerCase();
    if (elm == 'input' || elm == 'textarea' || elm == 'select' || elm == 'button') {
        return;
    }

    var event = null;
    if (evt.which == 73)      { event = 'inventory-toggle'; }
    else if (evt.which == 13) { event = 'chat-toggle'; }

    if (event !== null) {
        window.dispatchEvent(new Event(event));
    }
});