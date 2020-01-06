<template>
    <div class="chat-component bg-gray-800 text-gray-500 flex flex-col text-sm">
        <ul class="chat__messages w-full h-full overflow-y-scroll p-3">
            <li v-for="msg in msgs" v-bind:key="msg.id">{{msg}}</li>
        </ul>
        <div class="chat__input">
            <input
                class="w-full px-1 p-1 bg-gray-600 text-gray-900"
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
import Component from 'vue-class-component';
import {dataService} from '@/services/data.service';
import {SystemTypes} from "@/typegen";

@Component({ name: 'chat' })
export default class Chat extends Vue {
    public message: string = "";
    public msgs: any[] = [];

    created() {
        dataService.subscribe(SystemTypes.Location, (type: number, data: any) => {
            if (type === SystemTypes.LocationTypes.Chat) {
                this.msgs.push(data.message);
            }
        });

        window.addEventListener('chat-toggle', () => {
            let elm = document.querySelector('.chat__input input') as HTMLElement;
            if (elm !== null)
                elm.focus();
        });
    }

    send() {
        if (this.message.length > 0) {
            if (this.message.indexOf('/') == 0) {
                const commandArguments = this.message.split(' ');
                const commandType = commandArguments.shift();

                if (commandType === undefined)
                    return;

                dataService.send(SystemTypes.Command, SystemTypes.CommandTypes.Command, { command: commandType, arguments: commandArguments });
            } else {
                dataService.send(SystemTypes.Chat, SystemTypes.ChatTypes.Say, { message: this.message });
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