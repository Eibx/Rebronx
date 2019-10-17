<template>
    <div class="map-component bg-gray-900" ref="canvasContainer" v-on:mousemove="updateCoordinates" v-on:click="click"></div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import RenderService from '../services/render.service'
import MapService from '../services/map.service'

import WorldStore from '../stores/world.store';

@Component({ name: "world" })
export default class World extends Vue {
    $refs!: {
        canvasContainer: HTMLCanvasElement;
    };

    private from: number | null = null;
    private to: number | null = null;

    public worldStore = WorldStore;

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

    click() {
        let cursor = RenderService.cursorPosition;

        const location = MapService.getCloseLocation(cursor.x, cursor.y);

        if (location == null)
            return;

        let path = MapService.getShortestPath(this.worldStore.currentNode, location.id);

        MapService.startTravel(path);
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