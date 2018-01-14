<template>
	<div class="chat">
		<ul class="chat__messages">
			<li v-for="msg in msgs">{{msg}}</li>
		</ul>
		<div class="chat__input">
			<input type="text" v-on:keyup.enter="send" v-on:keyup.esc="blur" placeholder="type your message" v-model="message" />
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

		window.addEventListener('chat-toggle', () => {
			document.querySelector('.chat__input input').focus();
		});
	},
	methods: {
		send: function () {
			if (this.message.length > 0) {
				if (this.message.indexOf('/give') == 0) {
					var commandArguments = this.message.split(' ');
					commandArguments.shift();
					dataService.send('command', 'give', { arguments: commandArguments });
				} else {
					dataService.send('chat', 'say', { message: this.message });
				}
				
				this.message = "";
			} else {
				this.blur();
			}
		},
		blur: function () {
			document.querySelector('.chat__input input').blur();
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