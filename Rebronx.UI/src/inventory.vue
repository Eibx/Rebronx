<template>
	<div class="inventory" v-show="isVisible">
		<div class="inventory__container">
			<div class="inventory__equipment">
				<div class="inventory__equipment-armour">
					 <div class="inventory__equipment-armour-slot inventory__equipment-armour-head" v-on:mousedown="beginMove(10)" v-on:mouseup="endMove(10)">
						<div class="inventory__item" v-if="headSlot !== null">
							<div class="inventory__item-name">{{headSlot.name}}</div>
							<svg class="inventory__item-image" viewBox="0 0 100 100">
								<line x1="0" y1="0" x2="100" y2="100" stroke-width="2" stroke="#666" />
								<line x1="0" y1="100" x2="100" y2="0" stroke-width="2" stroke="#666" />
							</svg>
						</div>
					 </div>
					 <div class="inventory__equipment-armour-slot inventory__equipment-armour-body" v-on:mousedown="beginMove(11)" v-on:mouseup="endMove(11)">
						 <div class="inventory__item" v-if="bodySlot !== null">
							<div class="inventory__item-name">{{bodySlot.name}}</div>
							<svg class="inventory__item-image" viewBox="0 0 100 100">
								<line x1="0" y1="0" x2="100" y2="100" stroke-width="2" stroke="#666" />
								<line x1="0" y1="100" x2="100" y2="0" stroke-width="2" stroke="#666" />
							</svg>
						</div>
					 </div>
					 <div class="inventory__equipment-armour-slot inventory__equipment-armour-legs" v-on:mousedown="beginMove(12)" v-on:mouseup="endMove(12)">
						 <div class="inventory__item" v-if="legsSlot !== null">
							<div class="inventory__item-name">{{legsSlot.name}}</div>
							<svg class="inventory__item-image" viewBox="0 0 100 100">
								<line x1="0" y1="0" x2="100" y2="100" stroke-width="2" stroke="#666" />
								<line x1="0" y1="100" x2="100" y2="0" stroke-width="2" stroke="#666" />
							</svg>
						</div>
					 </div>
				</div>
				<div class="inventory__equipment-player">
					<div class="inventory__equipment-player-character"></div>
				</div>
				<div class="inventory__equipment-weapons">
					<div class="inventory__equipment-weapons-primary" v-on:mousedown="beginMove(20)" v-on:mouseup="endMove(20)">
						<div class="inventory__item" v-if="primaryWeapon !== null">
							<div class="inventory__item-name">{{primaryWeapon.name}}</div>
							<svg class="inventory__item-image" viewBox="0 0 100 100">
								<line x1="0" y1="0" x2="100" y2="100" stroke-width="2" stroke="#666" />
								<line x1="0" y1="100" x2="100" y2="0" stroke-width="2" stroke="#666" />
							</svg>
						</div>
					</div>
					<div class="inventory__equipment-weapons-secondary" v-on:mousedown="beginMove(30)" v-on:mouseup="endMove(30)">
						<div class="inventory__item" v-if="secondaryWeapon !== null">
							<div class="inventory__item-name">{{secondaryWeapon.name}}</div>
							<svg class="inventory__item-image" viewBox="0 0 100 100">
								<line x1="0" y1="0" x2="100" y2="100" stroke-width="2" stroke="#666" />
								<line x1="0" y1="100" x2="100" y2="0" stroke-width="2" stroke="#666" />
							</svg>
						</div>
					</div>
				</div>
			</div>
			<div class="inventory__slots">
				<div class="inventory__slot" v-for="(item, index) in items" v-on:mousedown="beginMove(index+100)" v-on:mouseup="endMove(index+100)">
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
			items: [],
			eqipment: [],
			moveItem: null,

			headSlot: null,
			bodySlot: null,
			legsSlot: null,

			primaryWeapon: null,
			primaryAmmo: null,
			secondaryWeapon: null,
			secondaryAmmo: null,
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
			var newItems = new Array(18);
			newItems.fill(null);
			this.headSlot = null;
			this.bodySlot = null;
			this.legsSlot = null;
			this.primaryWeapon = null;
			this.primaryAmmo = null;
			this.secondaryWeapon = null;
			this.secondaryAmmo = null;

			for (var i = 0; i < ids.length; i++) {
				var itemId = ids[i][0];
				var itemCount = ids[i][1];
				var itemSlot = ids[i][2];

				var item = this.getItem(itemId);
				var uiItem = null;
				if (item !== null) {
					uiItem = { name: item.name, count: itemCount };
				} else {
					uiItem = { name: "unknown " + itemId, count: itemCount };
				}

				if (itemSlot >= 100) {
					newItems[itemSlot-100] = uiItem;
				} else {

					if (itemSlot == 10) {
						this.headSlot = uiItem;
					} else if (itemSlot == 11) {
						this.bodySlot = uiItem;
					} else if (itemSlot == 12) {
						this.legsSlot = uiItem;
					} else if (itemSlot == 20) {
						this.primaryWeapon = uiItem;
					} else if (itemSlot == 21) {
						this.primaryAmmo = uiItem;
					} else if (itemSlot == 30) {
						this.secondaryWeapon = uiItem;
					} else if (itemSlot == 31) {
						this.secondaryAmmo = uiItem;
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

			return null;
		},
		beginMove(index) {
			if (index < 100) {
				this.moveItem = index;
			} else {
				for (var i = 0; i < this.items.length; i++) {
					if (this.items[index] === null) {
						this.moveItem = null;
						return;
					} else {
						this.moveItem = index;
					}
				}
			}
		},
		endMove(index) {
			if (this.moveItem === null) {
				return;
			}

			if (this.moveItem == index) {
				dataService.send('inventory', 'equip', { from: this.moveItem, to: null });
			} else {
				dataService.send('inventory', 'reorder', { from: this.moveItem, to: index });
			}

			this.moveItem = null;
		},
		unequip(slot) {
			alert(slot);
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

	width:200px;
	border:1px solid #666;
	margin-bottom:3px;
	margin-right:5px;
}
.inventory__equipment-weapons {
	display: flex;
	flex-direction: column;
	width:12%;
}

.inventory__equipment-weapons-primary {
	border:1px solid #666;
	margin-bottom:3px;	
}

.inventory__equipment-weapons-primary:after {
	display:block;
	content:'';
	padding-top:100%;
}

.inventory__equipment-weapons-secondary {
	border:1px solid #666;
	margin-bottom:3px;	
}

.inventory__equipment-weapons-secondary:after {
	display:block;
	content:'';
	padding-top:100%;
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