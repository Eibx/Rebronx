<template>
    <div class="lobby-component bg-gray-800">
        
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

<style scoped>
.lobby-component {
    width:360px;
    height:200px;

    position:absolute;
    top:20px;
    right:20px;
}
</style>