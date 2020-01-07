<template>
    <window
        v-if="showWindow"
        class="combat-window"
        title="Combat"
    >
        <div class="flex h-full py-2">
            <div class="flex-1 px-4">
                <div class="text-xs text-gray-400 mb-1">Select where to attack</div>
                <ol class="flex flex-row w-full">
                    <li class="flex-1 mr-2">
                        <div class="shadow-inner bg-gray-900 aspect-ratio-square cursor-pointer relative" @click="selectAttack(0)" :class="[attackPattern[0] ? 'bg-red-900' : 'bg-gray-900', opponentAtPosition(0) ? 'opponent-position' : '']"></div>
                    </li>
                    <li class="flex-1 mx-2">
                        <div class="shadow-inner bg-gray-900 aspect-ratio-square cursor-pointer relative" @click="selectAttack(1)" :class="[attackPattern[1] ? 'bg-red-900' : 'bg-gray-900', opponentAtPosition(1) ? 'opponent-position' : '']"></div>
                    </li>
                    <li class="flex-1 mx-2">
                        <div class="shadow-inner bg-gray-900 aspect-ratio-square cursor-pointer relative" @click="selectAttack(2)" :class="[attackPattern[2] ? 'bg-red-900' : 'bg-gray-900', opponentAtPosition(2) ? 'opponent-position' : '']"></div>
                    </li>
                    <li class="flex-1 ml-2">
                        <div class="shadow-inner bg-gray-900 aspect-ratio-square cursor-pointer relative" @click="selectAttack(3)" :class="[attackPattern[3] ? 'bg-red-900' : 'bg-gray-900', opponentAtPosition(3) ? 'opponent-position' : '']"></div>
                    </li>
                </ol>

                <div class="py-6 text-center">
                    <div class="text-xs leading-none">next round</div>
                    <div class="text-xl leading-none">{{nextRoundTimer.toFixed(1)}}s</div>
                </div>

                <div class="text-xs text-gray-400 mb-1">Select your position</div>
                <div class="flex flex-row w-full mb-4">
                    <combat-position class="flex-1 mr-2" :position="0" :current-position="position" />
                    <combat-position class="flex-1 mx-2" :position="1" :current-position="position" />
                    <combat-position class="flex-1 mx-2" :position="2" :current-position="position" />
                    <combat-position class="flex-1 ml-2" :position="3" :current-position="position" />
                </div>

                <ol class="flex flex-row w-full border-t-2 border-gray-900 pt-4">
                    <li class="w-8 h-8 shadow-inner bg-gray-900 mr-4">1</li>
                    <li class="w-8 h-8 shadow-inner bg-gray-900 mr-4">2</li>
                    <li class="w-8 h-8 shadow-inner bg-gray-900 mr-4">3</li>
                    <li class="w-8 h-8 shadow-inner bg-gray-900 mr-4">4</li>
                </ol>

            </div>
            <div class="flex-1 border-l-2 border-gray-900 px-4">
                <div class="text-xs text-gray-400 mb-1">Gamelog</div>
                <div class="shadow-inner bg-gray-900 p-4 text-xs">
                    <div v-for="log in combatLog">{{log}}</div>
                </div>
            </div>
        </div>
    </window>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import {Prop} from "vue-property-decorator";
    import Window from '@/components/shared/Window.vue'

    import {dataService} from '@/services/data.service'
    import {SystemTypes} from "@/typegen";
    import CombatPosition from "@/components/child/CombatPosition.vue";

    @Component({
        name: 'combat',
        components: {
            CombatPosition,
            Window
        },
    })
    export default class Combat extends Vue {
        public showWindow: boolean = true;

        public attackPattern: boolean[] = [];
        public position: number = 0;
        public fighters: any[] = [];

        public nextRound: number = 0;
        public nextRoundTimer: number = 0;

        public combatLog: string[] = [];

        created() {
            this.attackPattern = [false,false,false,false];

            dataService.subscribe(SystemTypes.Combat, (type:number, data:any) => {
                if (type === SystemTypes.CombatTypes.Report) {
                    this.showWindow = true;
                    this.nextRound = data.nextRound;

                    for (let i = 0; i < data.actions.length; i++) {
                        let action = data.actions[i];

                        let fighterId = action.fighter;
                        let fighterName = this.getFighterName(action.fighter);
                        let move = action.move;
                        let attacks = action.attacks;

                        for (let a = 0; a < attacks.length; a++) {
                            let attack = attacks[a];


                        }

                    }
                }

                if (type === SystemTypes.CombatTypes.EndFight) {
                    this.showWindow = false;
                    this.combatLog = [];
                    this.nextRound = 0;
                    this.nextRoundTimer = 0;
                }

                if (type === SystemTypes.CombatTypes.UpdateFight) {
                    this.fighters = data.fighters;
                }

                if (type === SystemTypes.CombatTypes.ChangeAttack) {
                    this.attackPattern = data.pattern;
                }

                if (type == SystemTypes.CombatTypes.ChangePosition) {
                    this.position = data.position;
                }
            });

            setInterval(() => {
                const d = new Date();
                let timeLeft = (this.nextRound - d.getTime()) / 1000;
                if (timeLeft <= 0) {
                    this.nextRoundTimer = 0;
                } else {
                    this.nextRoundTimer = timeLeft;
                }
            }, 100);
        }

        public getFighterName(id: number): string {
            let fighter = this.fighters.find(x => x.id == id);

            if (!fighter)
                return "";

            return fighter.name;
        }

        public opponentAtPosition(position: number): boolean {
            for (let i = 0; i < this.opponents.length; i++) {
                if (this.opponents[i].position == position)
                    return true;
            }

            return false;
        }

        public selectAttack(slot: number) {
            dataService.send(SystemTypes.Combat, SystemTypes.CombatTypes.ChangeAttack, {
                slot: slot,
                active: !this.attackPattern[slot]
            });
        }
    }
</script>

<style scoped>
    .combat-window {
        position:absolute;
        top:340px;
        left:20px;
        width:800px;
        height:600px;
    }

    .opponent-position::after {
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
</style>