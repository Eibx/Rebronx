<template>

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