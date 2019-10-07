<template>
    <div class="chat-component bg-gray-800 flex flex-col">
        <ul class="chat__messages w-full h-full">
            <li v-for="msg in msgs" v-bind:key="msg.id">{{msg}}</li>
        </ul>
        <div class="chat__input">
            <input
                class="w-full px-2 bg-gray-600"
                type="text"
                v-on:keyup.enter="send"
                v-on:keyup.esc="blur"
                placeholder="type your message"
                v-model="message"
                />
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import DataService from '../services/data.service'
import Component from 'vue-class-component';

@Component({ name: 'chat' })
export default class Chat extends Vue {
    public message: string = "";
    public msgs: any[] = [];

    created() {
        DataService.subscribe('lobby', (type: string, data: any) => {
            if (type === "chat") {
                this.msgs.push(data.message);
            }
        });

        window.addEventListener('chat-toggle', () => {
            var elm = document.querySelector('.chat__input input') as HTMLElement;
            if (elm !== null)
                elm.focus();
        });
    }
    
    send() {
        if (this.message.length > 0) {
            if (this.message.indexOf('/') == 0) {
                var commandArguments = this.message.split(' ');
                var commandType = commandArguments.shift();

                if (commandType === undefined)
                    return;

                DataService.send('command', commandType, { arguments: commandArguments });
            } else {
                DataService.send('chat', 'say', { message: this.message });
            }
            
            this.message = "";
        } else {
            this.blur();
        }
    }

    blur() {
        let elm = document.querySelector('.chat__input input') as HTMLElement;
        if (elm !== null)
            elm.blur();
    }
}
</script>

<style scoped>
.chat-component {
    width:360px;
    height:300px;

    position:absolute;
    bottom:20px;
    right:20px;
}
</style>