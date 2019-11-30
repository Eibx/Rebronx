<template>
    <div class="location-component bg-gray-800 p-4">
        <ul>
            <li
                class="bg-gray-700 px-2 py-1 text-gray-100 mb-2 cursor-pointer"
                v-for="player in players"
                @click="playerActiveContext = player.id"
                v-click-outside="hideContextMenu"
            >
                {{player.name}}

                <ul class="c-context-menu absolute border-2 border-gray-600 text-sm select-none" v-if="playerActiveContext && playerActiveContext === player.id">
                    <li class="bg-gray-800 hover:bg-gray-700 cursor-pointer border-b-2 border-gray-900 px-2" v-on:click="">Show player information</li>
                    <li class="bg-gray-800 hover:bg-gray-700 cursor-pointer border-b-2 border-gray-900 px-2">Message</li>
                    <li class="bg-red-800 hover:bg-red-900 cursor-pointer border-gray-900 px-2" v-show="!selectAttackMenu" @click="selectAttackMenu = true">Attack player</li>
                    <li class="cursor-pointer border-gray-900 flex" v-show="selectAttackMenu">
                        <div class="bg-gray-800 hover:bg-gray-700 flex-1 px-2" v-on:click="selectAttackMenu = false">Cancel</div>
                        <div class="bg-red-800 hover:bg-red-900 flex-1 px-2" v-on:click="selectAttackMenu = false">Attack</div>
                    </li>
                </ul>
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component';
    import {dataService} from "@/services/data.service";
    import ContextMenu from "@/components/shared/ContextMenu.vue";
    import {SystemTypes} from "@/typegen";

    @Component({
        name: "location",
        components: {
            ContextMenu
        }
    })
    export default class Location extends Vue {
        public players = [];

        public playerActiveContext: number | null = null;

        public selectAttackMenu: boolean = false;

        created() {
            dataService.subscribe(SystemTypes.Location, (type: number, data: any) => {
                if (type == SystemTypes.LocationTypes.PlayersUpdate) {
                    this.players = data.players;
                }
            });
        }

        public hideContextMenu() {
            this.playerActiveContext = null;
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

    .c-context-menu {
        box-shadow: 0 4px 4px rgba(0, 0, 0, 0.25);
    }
</style>