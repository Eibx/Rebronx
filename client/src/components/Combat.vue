<template>
    <window
        v-if="showWindow"
        class="combat-window"
        title="Combat"
    >
        <div class="flex h-full py-2">
            <div class="flex-1 px-4 py-2">
                <canvas ref="cv" width="400" height="400"></canvas>
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
    import CanvasHelper from "@/other/canvas-helper";

    @Component({
        name: 'combat',
        components: {
            CombatPosition,
            Window
        },
    })
    export default class Combat extends Vue {
        $refs!: {
            cv: HTMLCanvasElement;
        };

        public showWindow: boolean = true;

        public attackPattern: boolean[] = [];
        public position: number = 0;
        public fighters: any[] = [];

        public nextRound: number = 0;
        public nextRoundTimer: number = 0;

        public combatLog: string[] = [];

        private canvasHelper: CanvasHelper = new CanvasHelper();

        mounted() {
            //DEBUG
            this.$nextTick(() => {
                this.canvasHelper.setup(this.$refs.cv);
                this.renderCombat();
            });
            //DEBUG

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

                    this.$nextTick(() => {
                        this.canvasHelper.setup(this.$refs.cv);
                        this.renderCombat();
                    })
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
        }

        beforeDestroy() {
            this.canvasHelper.destroy();
        }

        private renderCombat() {
            let ch = this.canvasHelper;
            let ct = this.canvasHelper.context;

            ch.update();

            let w = 400;
            let h = 400;

            ct.clearRect(0, 0, w, h);

            let boxes = 4;
            let space = 20;
            let containerSize = (w+space)/boxes;
            let boxSize = containerSize-space;
            let distance = containerSize;

            for (let i = 0; i < boxes; i++) {
                let x = distance*i;
                let y = 0;

                let hover = ch.isHover(x, y, boxSize, boxSize);

                if (hover) {
                    this.$refs.cv.style.cursor = "pointer";
                }

                if (hover && ch.isMousePressed()) {
                    this.selectAttack(i);
                }

                ct.fillStyle = this.attackPattern[i] ? "#933" : "#999";
                ct.fillRect(x, y, boxSize, boxSize);
            }

            for (let i = 0; i < boxes; i++) {
                ct.fillStyle = "#999";
                ct.fillRect(distance*i, boxSize*2, boxSize, boxSize);
            }


            for (let i = 0; i < this.fighters.length; i++) {
                let fighter = this.fighters[i];

                let y = fighter.side == 0 ? 10 : 50;

                ct.fillStyle = "#333";
                ct.fillRect(distance+fighter.position, y, 10, 10);
            }

            ct.fillStyle = "#fff";
            ct.font = "12px monospace";
            let nextRoundWidth = ct.measureText("next round").width;
            ct.fillText("next round", (w-nextRoundWidth)/2, 115);

            ct.font = "20px monospace";
            let timeLeft = (this.nextRound - new Date().getTime()) / 1000;
            let timerWidth = ct.measureText(timeLeft.toFixed(1)).width;
            ct.fillText(timeLeft.toFixed(1), (w-timerWidth)/2, 140);
            ct.fillText(ch.globalMouseX + " " + ch.mouseX, 100, 160);

            if (this.showWindow)
                window.requestAnimationFrame(this.renderCombat);
        }

        public getFighterName(id: number): string {
            let fighter = this.fighters.find(x => x.id == id);

            if (!fighter)
                return "";

            return fighter.name;
        }

        public selectAttack(slot: number): void {
            dataService.send(SystemTypes.Combat, SystemTypes.CombatTypes.ChangeAttack, {
                slot: slot,
                active: !this.attackPattern[slot]
            });
        }

        public selectPosition(slot: number): void {
            dataService.send(SystemTypes.Combat, SystemTypes.CombatTypes.ChangePosition, {
                slot: slot
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