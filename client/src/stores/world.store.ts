import { VuexModule, Module, Mutation } from 'vuex-module-decorators'
import Vuex from 'vuex'
import Vue from "vue";

Vue.use(Vuex);

@Module
class WorldStoreModule extends VuexModule {
    currentNode: number = 0;
}

new Vuex.Store({
    modules: {
        myMod: WorldStoreModule
    }
});

export default WorldStoreModule