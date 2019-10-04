<template>
    <div class="map-component bg-gray-900" ref="canvasContainer" v-on:mousemove="updateCoordinates"></div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import DataService from '../services/data.service'
import RenderService from '../services/render.service'

@Component({ name: "world" })
export default class World extends Vue {
    $refs!: {
        canvasContainer: HTMLCanvasElement;
    };

    public mapUrl:string = "";

    created() {
        this.renderMap();

        window.addEventListener("resize", () => {
            RenderService.resize();
        });
    }

    async renderMap() {
        let canvas = await RenderService.setup();
        this.$refs.canvasContainer.appendChild(canvas);
    }

    updateCoordinates(event: MouseEvent) {
        RenderService.onMouseMove(event);
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