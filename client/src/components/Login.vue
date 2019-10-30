<template>
    <div class="login-component w-1/4 mx-auto pt-10">
        <h1 class="text-4xl text-gray-100">Rebronx</h1>
        <div class="flex flex-col mb-5">
            <div class="mb-1">
                <input
                    type="text"
                    placeholder="username"
                    v-model="username"
                    v-on:keyup.enter="login"
                    class="bg-gray-200 placeholder-gray-600 text-gray-900 px-1"
                />
            </div>
            <div class="mb-1">
                <input
                    type="password"
                    placeholder="password"
                    v-model="password"
                    v-on:keyup.enter="login"
                    class="bg-gray-200 placeholder-gray-600 text-gray-900 px-1"
                />
            </div>
            <div class="mb-1">
                <input
                    type="button"
                    v-on:click="login"
                    value="login"
                    class="bg-gray-200 text-gray-900 mr-1 px-1"
                />
                <input
                    type="button"
                    v-on:click="register"
                    value="register"
                    class="bg-gray-200 text-gray-900 mr-1 px-1"
                />
            </div>
        </div>
        <div v-if="showLoginIssue" class="mb-5 text-red-600 text-sm">
            Username and password doesn't match
        </div>
        <div v-if="showConnectionIssue" class="mb-5 text-red-600 text-sm">
            Cannot connect to server
        </div>
        <div v-if="isConnecting" class="mb-5 text-gray-600 text-sm">
            Connecting...
        </div>
        <div class="mb-5 text-gray-600 text-sm">
            You only need username and password to register
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import {loginService, LoginStatus} from '@/services/login.service'

@Component({ name: 'login' })
export default class Login extends Vue {
    public loginService = loginService;

    public username: string = "";
    public password: string = "";

    get isConnecting() {
        return this.loginService.state === LoginStatus.Connecting;
    }

    get showLoginIssue() {
        return this.loginService.state === LoginStatus.LoginError;
    }

    get showConnectionIssue() {
        return this.loginService.state === LoginStatus.ConnectionError;
    }

    login() {
        loginService.login(this.username, this.password);
    }

    register() {
        loginService.signup(this.username, this.password);
    }
}
</script>

<style scoped>
.login-component {
}
</style>