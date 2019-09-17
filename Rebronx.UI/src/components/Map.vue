<template>
    <div class="map-component bg-gray-900">
        <img :src="mapUrl" />
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import DataService from '../services/data.service'
import RenderService from '../services/render.service'

@Component({ name: "map" })
export default class Map extends Vue {
    public mapUrl:string = "";

    created() {
        RenderService.renderMap().then((url) => {
            this.mapUrl = url;
        });
    }
}
</script>

<style scoped>
.map-component {
    position:absolute;
    width:100%;
    height:100%;
    z-index:0;
}
</style>