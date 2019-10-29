<template>
    <div class="store">
        <ul class="store-items">
            <li class="store-item">
                <div class="store-item--image">
                    <img v-bind:src="image" alt="" />
                </div>
                <div class="store-item--info">
                    <div class="store-item--name">Knife</div>
                    <div class="store-item--buy">
                        <input class="store-item--buy-amount" type="text" value="1" />
                        <button class="store-item--buy-button" v-on:click="buy(4, 1)">$10</button>
                    </div>
                </div>
                <div class="infobox">
                    <h2>Knife</h2>
                    <p>This weapon can be used to stab people in the face</p>
                    <table>
                        <tr>
                            <td>Damage</td>
                            <td>105</td>
                        </tr>
                        <tr>
                            <td>Agility</td>
                            <td>15</td>
                        </tr>
                        <tr>
                            <td>Quality</td>
                            <td>Standard</td>
                        </tr>
                    </table>
                </div>
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import {dataService} from '@/services/data.service'

@Component({
    name: 'store'
})
export default class Store extends Vue {
    public image: string =  "";

    buy(item: any, amount: number) {
        dataService.send('store', 'buy', { store: 0, item: item, amount: amount });
    }
}
</script>

<style scoped>
</style>