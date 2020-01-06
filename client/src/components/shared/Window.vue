<template>
    <div class="c-window bg-gray-800 shadow flex flex-col">
        <div class="bg-gray-700 flex items-center px-2 text-gray-500">
            <div class="flex-1">{{title}}</div>
            <div>&#10005;</div>
        </div>
        <div class="text-gray-100 h-full">
            <slot />
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import {Prop} from "vue-property-decorator";

    @Component({
        name: 'window',
        components: {},
    })
    export default class Window extends Vue {
        @Prop({
            required: true,
            type: String
        })
        public title: string | undefined;
    }
</script>
<style>
</style>