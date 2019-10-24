<template>
    <div>
        <div v-if="!playerStore.isAuthenticated">
            <login />
        </div>

        <div v-else>
            <world />

            <lobby />
            <chat />
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import {Prop} from "vue-property-decorator";
    import Login from "@/components/Login.vue";
    import World from "@/components/World.vue";
    import Lobby from "@/components/Lobby.vue";
    import Chat from "@/components/Chat.vue";
    import playerStore from "@/stores/player.store";

    @Component({
        name: 'app',
        components: {
            Login,
            World,
            Lobby,
            Chat
        },
    })
    export default class App extends Vue {
        public playerStore = playerStore;
    }
</script>

<style scoped lang="less">
</style>