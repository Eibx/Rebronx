<template>
    <div class="w-800px p-16 mx-auto">
        <div class="bg-gray-800 py-8 mx-auto">
            <h1 class="text-3xl text-gray-200 px-8 mb-2">Rebronx</h1>
            <div class="flex">
                <div class="w-1/2 px-8 border-r">
                    <div class="flex flex-col mb-5">
                        <div class="mb-3">
                            <input
                                    type="text"
                                    placeholder="username"
                                    v-model="username"
                                    v-on:keyup.enter="login"
                                    class="w-full"
                            />
                        </div>
                        <div class="mb-3">
                            <input
                                    type="password"
                                    placeholder="password"
                                    v-model="password"
                                    v-on:keyup.enter="login"
                                    class="w-full"
                            />
                        </div>
                        <div class="mb-1 flex">
                            <input
                                    type="button"
                                    v-on:click="login"
                                    value="login"
                                    class="w-1/2 mr-3"
                            />
                            <input
                                    type="button"
                                    v-on:click="register"
                                    value="register"
                                    class="w-1/2"
                            />
                        </div>
                    </div>
                    <div v-if="showLoginIssue" class="mb-5 text-red-500 text-sm">
                        Username and password doesn't match
                    </div>
                    <div v-if="showConnectionIssue" class="mb-5 text-red-500 text-sm">
                        Cannot connect to server
                    </div>
                    <div v-if="isConnecting" class="mb-5 text-gray-600 text-sm">
                        Connecting...
                    </div>
                </div>
                <div class="w-1/2 px-8">
                    <div class="mb-5 text-gray-300 text-sm">
                        You only need username and password to register
                    </div>
                </div>
            </div>
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
</style>