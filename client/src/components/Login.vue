<template>
    <div class="login-component w-1/4 mx-auto pt-10" v-if="isVisible">
        <h1 class="text-4xl text-gray-100">Rebronx</h1>
        <div class="flex flex-col mb-5">
            <div class="mb-1">
                <input
                    type="text"
                    placeholder="username"
                    v-model="username"
                    v-on:keyup.enter="login"
                    class="bg-gray-400 px-1"
                />
            </div>
            <div class="mb-1">
                <input
                    type="password"
                    placeholder="password"
                    v-model="password"
                    v-on:keyup.enter="login"
                    class="bg-gray-400 px-1"
                />
            </div>
            <div class="mb-1">
                <input
                    type="button"
                    v-on:click="login"
                    value="login"
                    class="mr-1 px-1"
                />
                <input
                    type="button"
                    v-on:click="register"
                    value="register"
                    class="mr-1 px-1"
                />
            </div>
        </div>
        <div v-if="showErrorMessage" class="mb-5 text-red-600 text-sm">
            Username and password doesn't match
        </div>
        <div class="mb-5 text-gray-600 text-sm">
            You only need username and password to register
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import DataService from '../services/data.service'

@Component({ name: 'login' })
export default class Login extends Vue {
    public isVisible: boolean = true;
    public showErrorMessage: boolean = false;
    public username: string = "";
    public password: string = "";

    created() {
        DataService.subscribe('login', (type:string, data:any) => {
            if (data.success == true) {
                this.isVisible = false;
                window.localStorage.setItem("token", data.token);
            } else {
                this.showErrorMessage = true;
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
}
</style>