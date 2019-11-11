<template>
    <window
        class="inventory-window"
        title="Inventory"
    >
        Empty inventory
    </window>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import Window from '@/components/shared/Window.vue'

import {dataService} from '../services/data.service'


@Component({
    name: "inventory",
    components: {
        Window
    }
})
export default class Inventory extends Vue {

    created() {
        dataService.subscribe('inventory', (type:string, data:any) => {
            if (type == "update") {

            }
        });
    }

}
</script>
<style>
    .inventory-window {
        position:absolute;
        top:20px;
        left:20px;
        width:400px;
        height:300px;
    }
</style>