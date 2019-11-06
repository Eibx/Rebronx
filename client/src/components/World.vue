<template>
    <div class="map-component" ref="canvasContainer" v-on:mousemove="updateCoordinates" v-on:click="click"></div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import {renderService} from '@/services/render.service'
import {mapService} from '@/services/map.service'
import {worldStore} from '@/stores/world.store';

@Component({ name: "world" })
export default class World extends Vue {
    $refs!: {
        canvasContainer: HTMLCanvasElement;
    };

    private from: number | null = null;
    private to: number | null = null;

    mounted() {
        this.renderMap();

        window.addEventListener("resize", () => {
            renderService.resize();
        });
    }

    private renderMap() {
        let canvas = renderService.setup();
        this.$refs.canvasContainer.appendChild(canvas);
    }

    private updateCoordinates(event: MouseEvent) {
        renderService.onMouseMove(event);
    }

    click() {
        let cursor = renderService.cursorPosition;

        const location = mapService.getCloseLocation(cursor.x, cursor.y);

        if (location == null)
            return;

        let path = mapService.getShortestPath(worldStore.currentNode, location.id);

        mapService.startTravel(path);
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