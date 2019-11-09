<template>
    <div class="location-component bg-gray-800 p-4">
        <ul>
            <li
                class="bg-gray-700 px-2 py-1 text-gray-100 mb-2"
                v-for="player in players"
            >
                {{player.name}}
            </li>
        </ul>

        <div id="store"></div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component';
    import {dataService} from "@/services/data.service";

    @Component({ name: 'location' })
    export default class Location extends Vue {
        public players = [];

        created() {
            dataService.subscribe('location', (type: string, data: any) => {
                if (type == "location") {
                    this.players = data.players;
                }
            });
        }

    }
</script>

<style scoped>
    .location-component {
        width:360px;
        height:200px;

        position:absolute;
        top:20px;
        right:20px;
    }
</style>