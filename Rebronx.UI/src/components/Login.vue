<template>
    <div class="login-component" v-if="isVisible">
        <div class="flex flex-col">
            <div class="">
                <input type="text" placeholder="username" v-model="username" v-on:keyup.enter="login" />
            </div>
            <div class="">
                <input type="password" placeholder="password" v-model="password" v-on:keyup.enter="login" />
            </div>
            <div class="">
                <input type="button" v-on:click="login" value="login" />
                <input type="button" v-on:click="register" value="register" />
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import DataService from '../services/data.service'

@Component({ name: 'login' })
export default class Login extends Vue {
    public isVisible = true;
    public username = "";
    public password = "";
    
    created() {
        DataService.subscribe('login', (type:string, data:any) => {
            if (data.success == true) {
                this.isVisible = false;
                window.localStorage.setItem("token", data.token);
            }
        });

        var token = window.localStorage.getItem('token');
        if (token) {
            DataService.open((data: any) => {
                if (data.type == 'error') {
                    console.error('error connect to server');
                } else if (data.type == 'open') {
                    DataService.send('login', 'login', { token: token });
                    DataService.startPing();
                }
            });
        }
    }
    
    login() {
        var username = this.username;
        var password = this.password;

        DataService.open(function (data: any) {
            if (data.type == 'error') {
                console.error('error connect to server');
            } else if (data.type == 'open') {
                DataService.send('login', 'login', { username: username, password: password });
                DataService.startPing();
            }
        });
    }

    register() {
        var username = this.username;
        var password = this.password;

        DataService.open(function (data: any) {
            if (data.type == 'error') {
                console.error('error connect to server');
            } else if (data.type == 'open') {
                DataService.send('login', 'signup', { username: username, password: password });
                DataService.startPing();
            }
        });
    }
}
</script>

<style scoped>
.login-component {
    width:500px;
    height:400px;
    position:absolute;
    z-index: 100;
}
</style>