<template>
	<div class="inventory" v-show="isVisible">
		<div class="inventory__container">
			<div class="inventory__equipment">
				<div class="inventory__equipment-armour">
					 <div class="inventory__equipment-armour-slot inventory__equipment-armour-head"></div>
					 <div class="inventory__equipment-armour-slot inventory__equipment-armour-body"></div>
					 <div class="inventory__equipment-armour-slot inventory__equipment-armour-legs"></div>
				</div>
				<div class="inventory__equipment-player">
					<div class="inventory__equipment-player-character"></div>
				</div>
				<div class="inventory__equipment-weapons">
					<div class="inventory__equipment-weapons-primary"></div>
					<div class="inventory__equipment-weapons-secondary"></div>
				</div>
			</div>
			<div class="inventory__slots">
				<div class="inventory__slot" v-for="item in items">
					<div class="inventory__item" v-if="item !== null">
						<div class="inventory__item-name">{{item.name}}</div>
						<div class="inventory__item-count" v-if="item.count > 1">{{item.count}}</div>
						<svg class="inventory__item-image" viewBox="0 0 100 100">
							<line x1="0" y1="0" x2="100" y2="100" stroke-width="2" stroke="#666" />
							<line x1="0" y1="100" x2="100" y2="0" stroke-width="2" stroke="#666" />
						</svg>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
import DataService from './services/data.service.js'
export default {
	name: 'inventory',

	data() {
		return {
			isVisible: false,
			items: []
		}
	},
	created() {
		this.processItems([]);

		dataService.subscribe('inventory', (type, data) => {
			if (type == "update") {
				this.processItems(data);
			}
		});

		window.addEventListener('inventory-toggle', () => {
			this.isVisible = !this.isVisible;
		});
	},
	methods: {
		processItems(ids) {
			var newItems = [];
			for (var x = 0; x < 18; x++) {
				newItems[x] = null;

				for (var i = 0; i < ids.length; i++) {
					var invitem = ids[i];
					if (x == invitem[0]) {
						var item = this.getItem(invitem[1]);
						if (item !== undefined) {
							newItems[x] = { name: item.name, count: invitem[2] };
						} else {
							newItems[x] = { name: "unknown " + invitem[1], count: invitem[2] };
						}

					}
				}
			}
			this.items = newItems;
		},
		getItem(id) {
			for (var i = 0; i < window.itemData.data.length; i++) {
				var element = window.itemData.data[i];
				if (element.id == id)
					return element;
			}
		}
	}
}
</script>

<style>
.inventory {
	position: absolute;
	width: 100%;
	height: 100%;
	left: 0;
	top: 0;

	background: rgba(0, 0, 0, 0.6);

	z-index: 100;

	display:flex;
	position: absolute;
	align-items: center;
	justify-content: center;
}

.inventory__container {
	width: 600px;
	height: 600px;
	
	background: #fff;
	padding: 20px;

	display: flex;
	flex-direction: column;
}

.inventory__equipment {
	flex-grow: 1;

	display: flex;
	flex-direction: row;
}

.inventory__equipment-armour {
	display: flex;
	flex-direction: column;
	width:12%;

	margin-right: 5px;
}
.inventory__equipment-armour-slot {
	width:100%;
	border:1px solid #666;
	margin-bottom:3px;
}
.inventory__equipment-armour-slot:after {
	display:block;
	content:'';
	padding-top:100%;
}


.inventory__equipment-player {
	display: flex;
	flex-direction: column;

	width:100%;
	border:1px solid #666;
	margin-bottom:3px;
}
.inventory__equipment-weapons {
	display: flex;
	flex-direction: column;
}


.inventory__slots {
	display:flex;
	flex-wrap:wrap;
	border:1px solid #666;
	border-width:1px 0px 0px 1px;
}

.inventory__slot {
	width:calc(100% / 6);
	border:1px solid #666;
	border-width:0px 1px 1px 0px;
	margin:0;
	position:relative;
}
.inventory__slot:after {
	display:block;
	content:'';
	padding-top:100%;
}

.inventory__item {
	position:relative;
	top:0;
	left:0;
	width:100%;
	height:100%;
}

.inventory__item-image {
	position:absolute;
	top:0;
	left:0;
	width:100%;
	height:100%;
}

.inventory__item-name {
	position:absolute;
	padding:2px 5px;
	top:0;
	left:0;
	background:rgba(0, 0, 0, 0.6);
	z-index:105;
}

.inventory__item-count {
	position:absolute;
	padding:2px 5px;
	right:0;
	bottom:0;
	background:rgba(0, 0, 0, 0.5);
	z-index:105;
}
</style>