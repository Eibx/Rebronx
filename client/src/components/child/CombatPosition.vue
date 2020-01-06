<template>
    <div class="flex-1 mr-2">
        <div
            class="shadow-inner bg-gray-900 aspect-ratio-square relative"
            @click="changePosition()"
            :class="{ 'current-position': isCurrentPosition }">
            <div class="damage-indicator">20 <span class="text-xs">(25%)</span></div>
            <div class="miss-indicator text-xs">miss</div>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import {Prop} from "vue-property-decorator";
    import {dataService} from '@/services/data.service'
    import {SystemTypes} from "@/typegen";

    @Component({
        name: 'combat-position',
        components: {},
    })
    export default class CombatPosition extends Vue {
        @Prop({
            required: true,
            type: Number
        })
        public position!: number;

        @Prop({
            required: true,
            type: Number
        })
        public currentPosition!: number;

        get isCurrentPosition() {
            return this.position === this.currentPosition;
        }

        public changePosition() {
            dataService.send(SystemTypes.Combat, SystemTypes.CombatTypes.ChangePosition, {
                position: this.position
            });
        }
    }
</script>

<style scoped>
    .current-position::after {
        content:'';
        display:block;
        background-color:#FFF;
        position:absolute;
        left:50%;
        top:50%;
        margin-left:-10px;
        margin-top:-10px;
        border-radius: 20px;
        width:20px;
        height:20px;
    }

    @keyframes damage-pop {
        0% {
            top: 0;
            opacity: 1;
        }

        75% {
            top:-30px;
            opacity: 1;
        }

        100% {
            top:-35px;
            opacity: 0;
        }
    }

    .damage-indicator {
        transition: all;
        position: absolute;
        width:100%;
        text-align: center;
        opacity: 1;
        font-weight: bold;
        color:red;
        animation: ease-out damage-pop 3s infinite;
        text-shadow: 0 0 5px rgba(0, 0, 0, 1);
    }

    .miss-indicator {
        transition: all;
        position: absolute;
        width:100%;
        text-align: center;
        opacity: 1;
        font-weight: bold;
        color:gray;
        animation: ease-out damage-pop 3s infinite;
        text-shadow: 0 0 5px rgba(0, 0, 0, 1);
    }
</style>