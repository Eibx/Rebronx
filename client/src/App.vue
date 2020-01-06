<template>
    <div>
        <div v-if="!playerStore.isAuthenticated">
            <login />
        </div>

        <div v-else>
            <world />

            <location />
            <chat />
            <inventory />
            <combat />
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import Login from "@/components/Login.vue";
    import World from "@/components/World.vue";
    import Location from "@/components/Location.vue";
    import Chat from "@/components/Chat.vue";
    import {playerStore} from "@/stores/player.store";
    import Inventory from "@/components/Inventory.vue";
    import Combat from "@/components/Combat.vue";

    @Component({
        name: 'app',
        components: {
            Inventory,
            Login,
            World,
            Location,
            Chat,
            Combat
        },
    })
    export default class App extends Vue {
        public playerStore = playerStore;
    }
</script>