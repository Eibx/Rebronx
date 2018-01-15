import Vue from 'vue'
import Chat from './chat.vue'
import Login from './login.vue'
import Map from './map.vue'
import Lobby from './lobby.vue'
import Inventory from './inventory.vue'
import Store from './store.vue'

import DataService from './services/data.service.js'
import PlayerService from './services/player.service.js'
import RenderService from './services/render.service.js'

window.mapData = require("../../Rebronx.Data/map.json");
window.itemData = require("../../Rebronx.Data/items.json");

window.dataService = new DataService();
window.playerService = new PlayerService();
window.renderService = new RenderService();

Vue.directive('show-infobox', function (el, binding) {
	el.addEventListener('mousemove', function(e) {
		var infobox = el.parentNode.querySelector('.infobox');

		infobox.style.display = 'block';
		infobox.style.left = e.clientX + 10 + 'px';
		infobox.style.top = e.clientY + 10 + 'px';
	});
	
	el.addEventListener('mouseout', function(e) {
		var infobox = el.parentNode.querySelector('.infobox');
		infobox.style.display = 'none';
	});
});

new Vue({ el: '#chat', render: r => r(Chat) });
new Vue({ el: '#login', render: r => r(Login) });
new Vue({ el: '#map', render: r => r(Map) });
new Vue({ el: '#lobby', render: r => r(Lobby) });
new Vue({ el: '#inventory', render: r => r(Inventory) });
new Vue({ el: '#store', render: r => r(Store) });

window.addEventListener('keyup', function (evt) {
	var elm = evt.target.tagName.toLowerCase();
	if (elm == 'input' || elm == 'textarea' || elm == 'select' || elm == 'button') {
		return;
	}

	var event = null;
	if (evt.which == 73) { event = 'inventory-toggle'; }
	else if (evt.which == 13) { event = 'chat-toggle'; }

	if (event !== null) {
		window.dispatchEvent(new Event(event));
	}
});