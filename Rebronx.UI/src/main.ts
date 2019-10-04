import Vue from 'vue';
import Chat from './components/Chat.vue';
import Login from './components/Login.vue';
import World from './components/World.vue';
import Inventory from './components/Inventory.vue';
import AreaOverview from './components/Area.vue';
import Store from './components/Store.vue';

Vue.config.productionTip = false

new Vue({ el: 'chat', render: r => r(Chat) });
new Vue({ el: 'login', render: r => r(Login) });
new Vue({ el: 'world', render: r => r(World) });
//new Vue({ el: 'inventory', render: r => r(Inventory) });
//new Vue({ el: 'area-overview', render: r => r(AreaOverview) });
//new Vue({ el: 'store', render: r => r(Store) });

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