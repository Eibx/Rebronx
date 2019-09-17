import Vue from 'vue';
import Chat from './components/Chat.vue';
import Login from './components/Login.vue';
import Map from './components/Map.vue';
import Lobby from './components/Lobby.vue';
import Inventory from './components/Inventory.vue';
import Area from './components/Area.vue';
import Store from './components/Store.vue';

Vue.config.productionTip = false

new Vue({ el: 'chat', render: r => r(Chat) });
new Vue({ el: 'login', render: r => r(Login) });
new Vue({ el: 'map', render: r => r(Map) });
new Vue({ el: 'lobby', render: r => r(Lobby) });
new Vue({ el: 'inventory', render: r => r(Inventory) });
new Vue({ el: 'area', render: r => r(Area) });
new Vue({ el: 'store', render: r => r(Store) });

window.addEventListener('keyup', function (evt: KeyboardEvent) {
    if (evt === null || evt.target === null)
        return;

    let elm = evt.target as HTMLElement;
    let tag = elm.tagName;
    if (tag === 'input' || tag === 'textarea' || tag === 'select' || tag === 'button') {
        return;
    }

    let event = null;
    if (evt.which == 73) { event = 'inventory-toggle'; }
    else if (evt.which == 13) { event = 'chat-toggle'; }

    if (event !== null) {
        window.dispatchEvent(new Event(event));
    }
});