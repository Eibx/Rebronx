import {VNode} from "vue";

class HTMLHandlerElement extends HTMLElement {
    public clickOutsideHandler: any;

    constructor() {
        super();
    }
}

class ClickOutsideDirective {
    public bind(el: HTMLHandlerElement, binding: any, vnode: any) {
        console.log('bind?');
        const bubble = binding.modifiers.bubble;
        const handler = (evt: MouseEvent) => {
            if (bubble || (!el.contains(evt.target as any) && el !== evt.target)) {
                binding.value(evt);
            }
        };

        el.clickOutsideHandler = handler;
        document.addEventListener('click', handler)
    }

    public unbind(el: HTMLHandlerElement, binding: any) {
        document.removeEventListener('click', el.clickOutsideHandler);
        el.clickOutsideHandler = null;
    }
}