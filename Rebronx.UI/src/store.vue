<template>
	<div class="store">
		<ul class="store-items">
			<li class="store-item">
				<div class="store-item--image" v-show-infobox>
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

<script>
import DataService from './services/data.service.js'
import RenderService from './services/render.service.js'
export default {
	name: 'area',

	data() {
		return {
			image: "",
		}
	},
	created() {
		renderService.renderItem(4).then((src) => { this.image = src; });
	},
	methods: {
		buy: function(item, amount) {
			dataService.send('store', 'buy', { store: 0, item: item, amount: amount });
		}
	}
}
</script>

<style scoped>
.store-items {
	display: flex;
	flex-direction: row;
	flex-wrap: wrap;
}
.store-item {
	width: calc(33.33% - 10px);

	padding:5px;

	margin:5px;
	border:1px solid #393F4C;
	background: #171b1f;

	display:flex;
}
.store-item--image {
	width:25%;
	height:60px;
	margin-right:5px;
}
.store-item--image img {
	width:100%;
}
.store-item--info {
	width:calc(75% - 5px);
	display: flex;
	flex-direction: column;
}
.store-item--name {
	flex-grow: 1;
}
.store-item--buy {
	
}
.store-item--buy-amount {
	width:40px;
}
.store-item--buy-button {

}


</style>