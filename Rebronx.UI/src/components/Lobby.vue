<template>
    <div class="h-full bg-gray-800" style="width:340px;">
        
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import DataService from '../services/data.service'
import Component from 'vue-class-component';

@Component({ name: 'lobby' })
export default class Lobby extends Vue {
    public players: any[] = [];

    created() {
        var self = this;
        DataService.subscribe('lobby', (type: string, data: any) => {
            if (type == "lobby") {
                this.players = data.players;
            }
        });
    }
}
</script>