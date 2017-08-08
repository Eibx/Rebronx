<template>
	<div class="login" v-if="isVisible">
		{{isVisible}}
		<div class="login__container">
			<div class="login__field">
				<input type="text" placeholder="username" v-model="username" v-on:keyup.enter="login" />
			</div>
			<div class="login__field">
				<input type="password" placeholder="password" v-model="password" v-on:keyup.enter="login" />
			</div>
			<div class="login__field">
				<input type="button" v-on:click="login" value="login" />
			</div>
		</div>
	</div>
</template>

<script>
import DataService from './services/data.service.js'
export default {
	name: 'login',

	data() {
		return {
			isVisible: true,
			username: "",
			password: "",
		}
	},
	created() {
		var self = this;

		dataService.subscribe('login', function (type, data) {
			if (data.success == true) {
				self.isVisible = false;
				window.localStorage.setItem("token", data.token);
			}
		});

		var token = window.localStorage.getItem('token');
		if (token) {
			dataService.open(function (data) {
				if (data.type == 'error') {
					alert('error connect to server');
				} else if (data.type == 'open') {
					dataService.send('login', 'login', { token: token });
					dataService.startPing();
				}
			});			
		}
	},
	methods: {
		login: function () {
			var username = this.username;
			var password = this.password;

			dataService.open(function (data) {
				if (data.type == 'error') {
					alert('error connect to server');
				} else if (data.type == 'open') {
					dataService.send('login', 'login', { username: username, password: password });
					dataService.startPing();
				}
			});
		}
	}
}
</script>

<style scoped>
.login {
	position: absolute;
	width: 100%;
	height: 100%;
	left: 0;
	top: 0;v-if="isVisible"

	background: rgba(0, 0, 0, 0.6);

	z-index: 100;
}

.login__container {
	position: absolute;
	width: 300px;
	height: 200px;
	left: 50%;
	top: 50%;
	margin-top: -100px;
	margin-left: -150px;

	background: #fff;

	padding: 20px;
}

.login__field {
	margin-bottom: 15px;
}

.login__field input {
	width:100%;
}
</style>
