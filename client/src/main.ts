import Vue, {VNode} from 'vue';

import App from "@/App.vue";

import {playerService} from "@/services/player.service";
import {loginService} from "@/services/login.service";

Vue.config.productionTip = false;

loginService.setup();
playerService.setup();

new Vue({ el: 'app', render: r => r(App) });

let handleOutsideClick: any;
Vue.directive('click-outside', {
    bind (el:any, binding:any, vnode:any) {
        handleOutsideClick = (e:any) => {
            e.stopPropagation();

            if (!el.contains(e.target)) {
                binding.value();
            }

        };

        document.addEventListener('click', handleOutsideClick);
        document.addEventListener('touchstart', handleOutsideClick);
    },

    unbind () {
        document.removeEventListener('click', handleOutsideClick);
        document.removeEventListener('touchstart', handleOutsideClick);
    }
});

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