<template>
    <div class="map-component bg-gray-900" ref="canvasContainer" v-on:mousemove="updateCoordinates" v-on:click="click"></div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import DataService from '../services/data.service'
import RenderService from '../services/render.service'
import MapService from '../services/map.service'

@Component({ name: "world" })
export default class World extends Vue {
    $refs!: {
        canvasContainer: HTMLCanvasElement;
    };

    public mapUrl:string = "";

    private from: number | null = null;
    private to: number | null = null;

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

        var location = MapService.getCloseLocation(cursor.x, cursor.y);

        if (location == null)
            return;

        if (this.from === null) {
            this.from = location.id;
        } else if (this.to === null) {
            this.to = location.id;

            let path = MapService.getShortestPath(this.from, this.to)
            
            MapService.setActivePath(path);
            RenderService.setActivePath(path);

            this.from = null;
            this.to = null;
        }
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