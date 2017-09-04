<template>
	<div class="chat">
		<ul class="chat__messages">
			<li v-for="msg in msgs">{{msg}}</li>
		</ul>
		<div class="chat__input">
			<input type="text" v-on:keyup.enter="send" placeholder="type your message" v-model="message" />
		</div>
	</div>
</template>

<script>
import DataService from './services/data.service.js'
export default {
	name: 'chat',

	data() {
		return {
			msgs: [],
			message: ""
		}
	},
	created() {
		dataService.subscribe('lobby', (type, data) => {
			if (type == "chat") {
				this.msgs.push(data.message);
			}
		});
	},
	methods: {
		send: function () {
			dataService.send('chat', 'say', { message: this.message });
			this.message = "";
		}
	}
}
</script>

<style scoped>
.chat {
	position: absolute;
	left: 20px;
	bottom: 20px;
	z-index:10;
}

.chat__messages {
	margin-bottom: 10px;
	width:400px;
	word-break:break-all;
}
</style>
