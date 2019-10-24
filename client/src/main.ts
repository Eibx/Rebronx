import Vue from 'vue';

import PlayerService from './services/player.service';

import App from "@/App.vue";

Vue.config.productionTip = false;

PlayerService.setup();

new Vue({ el: 'app', render: r => r(App) });

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