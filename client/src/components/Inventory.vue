<template>
    <div class="c-window inventory-window">
        <div class="c-window__bar">Inventory</div>
        <div class="c-window__content">
            Empty inventory
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'

import {dataService} from '../services/data.service'

@Component({ name: "inventory" })
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