import Vue from 'vue';

import App from "@/App.vue";

import {playerService} from './services/player.service';
import {loginService} from "@/services/login.service";

Vue.config.productionTip = false;

loginService.setup();
playerService.setup();

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